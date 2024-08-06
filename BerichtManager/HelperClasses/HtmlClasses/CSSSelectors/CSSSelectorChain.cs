using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BerichtManager.HelperClasses.HtmlClasses.CSSSelectors
{
	/// <summary>
	/// Class used to search an <see cref="HtmlElement"/> using CSS
	/// </summary>
	internal class CSSSelectorChain
	{
		/// <summary>
		/// CSS selector to search with
		/// </summary>
		public string Selector { get; private set; }
		/// <summary>
		/// Root <see cref="HtmlElement"/> to search
		/// </summary>
		public HtmlElement Root { get; }
		/// <summary>
		/// <see cref="TimeSpan"/> after which <see cref="Regex"/> will default
		/// </summary>
		private TimeSpan RegexTimeOut { get; } = new TimeSpan(0, 1, 0);
		/// <summary>
		/// Root selector to start search at
		/// </summary>
		private BaseSelector RootSelector { get; set; }

		/// <summary>
		/// Creates a new <see cref="CSSSelectorChain"/> object
		/// </summary>
		/// <param name="cssSelector">CSS selector to search with</param>
		/// <param name="root">Root <see cref="HtmlElement"/> to search</param>
		public CSSSelectorChain(string cssSelector, HtmlElement root)
		{
			Selector = cssSelector;
			Root = root;
			Sanitize();
			BuildChain();
		}

		/// <summary>
		/// Removes hindering whitespace
		/// </summary>
		private void Sanitize()
		{
			//Remove hindering spaces
			Selector = Regex.Replace(Selector, @"(( +?)?(?=>)>( +?)?(?=[a-zA-z0-9]|$|\.))", ">", RegexOptions.None, RegexTimeOut);
			Selector = Regex.Replace(Selector, @"( +?(?=[a-zA-z0-9]|$|\.|#|$))", " ", RegexOptions.None, RegexTimeOut);
		}

		/// <summary>
		/// Builds the chain of CSS selectors
		/// </summary>
		/// <exception cref="UnhandledTypeException"></exception>
		private void BuildChain()
		{
			//Check which selector should be used with which css part
			List<(Type Type, string selector)> toBuild = new List<(Type, string)>();
			string newSelector = "";
			char combinator = ' ';
			for (int i = 0; i < Selector.Length; i++)
			{
				switch (Selector[i])
				{
					case ' ':
					case '>':
						if (combinator == ' ')
							toBuild.Add((typeof(Selector), newSelector));
						else
							toBuild.Add((typeof(ChildSelector), newSelector));
						newSelector = "";
						combinator = Selector[i];
						break;
					default:
						newSelector += Selector[i];
						break;
				}
			}
			if (combinator == ' ')
				toBuild.Add((typeof(Selector), newSelector));
			else
				toBuild.Add((typeof(ChildSelector), newSelector));
			toBuild.Reverse();

			//Build chain based on list of types
			List<BaseSelector> chain = new List<BaseSelector>();
			foreach ((Type Type, string selector) tuple in toBuild)
			{
				HtmlElement root = null;
				if (tuple == toBuild.Last())
					root = Root;
				BaseSelector toAdd;
				if (tuple.Type == typeof(ChildSelector))
				{
					if (chain.Count == 0)
						toAdd = new ChildSelector(tuple.selector, null, root);
					else
						toAdd = new ChildSelector(tuple.selector, chain.Last(), root);
				}
				else if (tuple.Type == typeof(Selector))
				{
					if (chain.Count == 0)
						toAdd = new Selector(tuple.selector, null, root);
					else
						toAdd = new Selector(tuple.selector, chain.Last(), root);
				}
				else
				{
					throw new UnhandledTypeException(tuple.Type);
				}
				chain.Add(toAdd);
			}
			RootSelector = chain.LastOrDefault();
		}

		/// <summary>
		/// Starts the CSS selection
		/// </summary>
		/// <returns><see cref="List{T}"/> of <see cref="HtmlElement"/>s that fit the provided selector</returns>
		public List<HtmlElement> StartSearch()
		{
			return RootSelector.Search(null);
		}
	}
}
