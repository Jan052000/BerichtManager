using Word = Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using BerichtManager.HelperClasses;
using BerichtManager.Extensions;
using System.Linq;
using BerichtManager.OwnControls;

namespace BerichtManager.WordTemplate
{
	public class FormFieldHandler
	{
		/// <summary>
		/// Path to directory containing config file
		/// </summary>
		private static string ConfigFolderPath { get; } = Path.Combine(Environment.CurrentDirectory, "Config");
		/// <summary>
		/// Name of the config file
		/// </summary>
		private static string ConfigFileName { get; } = "FormFieldConfig.json";
		/// <summary>
		/// Full path of config file
		/// </summary>
		private static string FormFieldConfigPath { get; } = Path.Combine(ConfigFolderPath, ConfigFileName);

		/// <summary>
		/// <see cref="Type"/> of <see cref="int"/>
		/// </summary>
		private static Type Int32Type { get; } = typeof(int);
		/// <summary>
		/// <see cref="Type"/> of <see cref="string"/>
		/// </summary>
		private static Type StringType { get; } = typeof(string);
		/// <summary>
		/// <see cref="Type"/> of <see cref="DateTime"/>
		/// </summary>
		private static Type DateTimeType { get; } = typeof(DateTime);
		/// <summary>
		/// <see cref="Type"/> of <see cref="bool"/>
		/// </summary>
		private static Type BooleanType { get; } = typeof(bool);

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> containing form field order and <see cref="Type"/>s
		/// Note that <see cref="Word.FormFields"/> start at index <c>1</c>
		/// </summary>
		private Dictionary<Fields, FormField> FormFields { get; set; } = GetInitialConfig();

		/// <summary>
		/// Holds all known <see cref="Fields"/> and their respective <see cref="Type"/>s
		/// </summary>
		private static Dictionary<Fields, Type> FieldTypes
		{
			get => new Dictionary<Fields, Type>()
			{
				{Fields.Name, Int32Type },
				{Fields.Number, Int32Type },
				{Fields.StartDate, DateTimeType },
				{Fields.EndDate, DateTimeType },
				{Fields.Year, Int32Type },
				{Fields.Work, StringType },
				{Fields.Seminars, StringType },
				{Fields.School, StringType },
				{Fields.SignDateYou, DateTimeType },
				{Fields.SignDateSupervisor, DateTimeType }
			};
		}

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> to switch an <see cref="object"/> to a respective <see cref="Type"/>
		/// </summary>
		private Dictionary<Type, Func<string, object>> TypeSwitchDict { get; } = new Dictionary<Type, Func<string, object>>()
		{
			{typeof(int), (o) => int.Parse(o)},
			{typeof(string), (o) => o},
			{typeof(DateTime), (o) => DateTime.ParseExact(o, "dd.MM.yyyy", new CultureInfo("de-DE"))},
			{typeof(bool), (o) => bool.Parse(o)}
		};

		#region Singleton
		private static FormFieldHandler Singleton { get; set; }
		private static FormFieldHandler Instance
		{
			get
			{
				if (Singleton == null)
					Singleton = new FormFieldHandler();
				return Singleton;
			}
		}
		#endregion

		private FormFieldHandler()
		{
			//Load from config
			Load();
			AlertForDoubleIndex();
		}

		/// <summary>
		/// Generates the default form fields config
		/// </summary>
		/// <returns>Default form fields config</returns>
		private static Dictionary<Fields, FormField> GetInitialConfig()
		{
			return new Dictionary<Fields, FormField>()
			{
				{Fields.Name, new FormField(1, FieldTypes[Fields.Name]) },
				{Fields.Number, new FormField(2, FieldTypes[Fields.Number]) },
				{Fields.StartDate, new FormField(3, FieldTypes[Fields.StartDate]) },
				{Fields.EndDate, new FormField(4, FieldTypes[Fields.EndDate]) },
				{Fields.Year, new FormField(5, FieldTypes[Fields.Year]) },
				{Fields.Work, new FormField(6, FieldTypes[Fields.Work]) },
				{Fields.Seminars, new FormField(7, FieldTypes[Fields.Seminars]) },
				{Fields.School, new FormField(8, FieldTypes[Fields.School]) },
				{Fields.SignDateYou, new FormField(9, FieldTypes[Fields.SignDateYou]) },
				{Fields.SignDateSupervisor, new FormField(10, FieldTypes[Fields.SignDateSupervisor]) }
			};
		}

		/// <summary>
		/// Gets the index of a <see cref="Word.FormField"/> in the template config
		/// </summary>
		/// <param name="field"><see cref="Fields"/> value of <see cref="Word.FormField"/> to get index of</param>
		/// <returns>Index of <paramref name="field"/> in the template config or <see langword="null"/> if <paramref name="field"/> was not found</returns>
		public static int? GetFormFieldIndex(Fields field)
		{
			if (!GetFormField(field, out FormField info))
				return null;
			return info.Index;
		}

		/// <summary>
		/// Gets the <see cref="FormField"/> object containing field index and type
		/// </summary>
		/// <param name="field"><see cref="Fields"/> field to get info of</param>
		/// <returns><see cref="FormField"/> object containing field index and type or <see langword="null"/> if <paramref name="field"/> was not found</returns>
		public static bool GetFormField(Fields field, out FormField info)
		{
			info = null;
			if (!Instance.FormFields.TryGetValue(field, out FormField _info))
				return false;
			info = _info;
			return true;
		}

		/// <summary>
		/// Gets and transforms the value of <paramref name="field"/> from <paramref name="doc"/> and tries to convert it to <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"><see cref="Type"/> to convert result to</typeparam>
		/// <param name="field"><see cref="Fields"/> field to get from <paramref name="doc"/></param>
		/// <param name="doc"><see cref="Word.Document"/> to get value of <paramref name="field"/> from</param>
		/// <returns>Value of <paramref name="field"/> converted to <typeparamref name="T"/> or <see langword="null"/> if no conversion is possible</returns>
		public static T GetValueFromDoc<T>(Fields field, Word.Document doc)
		{
			if (!GetFormField(field, out FormField info) || !Instance.TypeSwitchDict.TryGetValue(info.FieldType, out Func<string, object> convert))
				return (T)(object)null;
			if (typeof(T) == typeof(string))
				return (T)Instance.TypeSwitchDict[typeof(string)](doc.FormFields[info.Index].Result);
			return (T)convert(doc.FormFields[info.Index].Result);
		}

		/// <summary>
		/// Sets the value of the <paramref name="field"/> in <paramref name="doc"/> to be <paramref name="value"/>
		/// </summary>
		/// <param name="field"><see cref="Fields"/> field to set value</param>
		/// <param name="doc"><see cref="Word.Document"/> to set value of <paramref name="field"/> in</param>
		/// <param name="value">Value to set <paramref name="field"/> to</param>
		public static void SetValueInDoc(Fields field, Word.Document doc, string value)
		{
			if (!GetFormField(field, out FormField info))
				return;
			FillFormField(doc.Application, doc.FormFields[info.Index], value);
		}

		/// <summary>
		/// Checks if <paramref name="doc"/> has the correct amount of form fields
		/// </summary>
		/// <param name="doc"><see cref="Word.Document"/> to check form field count of</param>
		/// <returns><see langword="true"/> if <paramref name="doc"/> fulfills the form field count requirement and <see langword="false"/> otherwise</returns>
		public static bool ValidFormFieldCount(Word.Document doc)
		{
			return doc.FormFields.Count == Instance.FormFields.Count;
		}

		/// <summary>
		/// Loads data from file at <see cref="FormFieldConfigPath"/> and overwrites <see cref="FormFields"/> if necessary
		/// </summary>
		private void Load()
		{
			if (!File.Exists(FormFieldConfigPath))
				return;
			Dictionary<Fields, FormField> _FormFields = JsonConvert.DeserializeObject<Dictionary<Fields, FormField>>(File.ReadAllText(FormFieldConfigPath));
			if (!_FormFields.KeyValuePairsEqualNoSequence(FormFields))
				FormFields = _FormFields;
		}

		/// <summary>
		/// Resets form field config to default and deletes config file
		/// </summary>
		public static void ResetConfig()
		{
			Instance.FormFields = GetInitialConfig();
			if (File.Exists(FormFieldConfigPath))
				File.Delete(FormFieldConfigPath);
		}

		/// <summary>
		/// Checks <see cref="FormFields"/> for multiple of same index causing possible problems with overwriting
		/// </summary>
		private void AlertForDoubleIndex()
		{
			bool multipleSameIndex = FormFields.GroupBy(x => x.Value.Index).Where(x => x.Count() > 1).Count() > 0;
			if (multipleSameIndex)
				ThemedMessageBox.Show(text: "You have multiple form fields at the same index, they may overwrite eachother, please fix the form field config", title: "Multiple of same index detected");
		}

		/// <summary>
		/// Updates index of <paramref name="field"/> to <paramref name="newIndex"/>
		/// </summary>
		/// <param name="field"><see cref="Fields"/> field to update index of</param>
		/// <param name="newIndex">New index of <see cref="Fields"/> field in Word template</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="newIndex"/> < 1</exception>
		public static void UpdateFormFieldIndex(Fields field, int newIndex)
		{
			if (newIndex < 1)
				throw new ArgumentException($"{newIndex} is an invalid index, Word form fields start at index 1", "newIndex");
			if (!Instance.FormFields.TryGetValue(field, out FormField form))
			{
				FormField newForm = new FormField(index: newIndex, type: FieldTypes[field]);
				Instance.FormFields.Add(field, newForm);
			}
			if (form?.Index == newIndex)
				return;
			if (form != null)
				form.Index = newIndex;
			SaveConfig();
		}

		/// <summary>
		/// Sorts <see cref="FormFields"/> and saves it at to <see cref="FormFieldConfigPath"/>
		/// </summary>
		private static void SaveConfig()
		{
			SortFormFields();
			if (!Directory.Exists(ConfigFolderPath))
				Directory.CreateDirectory(ConfigFolderPath);
			File.WriteAllText(FormFieldConfigPath, JsonConvert.SerializeObject(Instance.FormFields));
		}

		/// <summary>
		/// Sorts <see cref="FormFields"/> by field index
		/// </summary>
		private static void SortFormFields()
		{
			Instance.FormFields = Instance.FormFields.OrderBy(kvp => kvp.Value.Index).ToDictionary(x => x.Key, x => x.Value);
		}

		/// <summary>
		/// Deletes <paramref name="field"/> from config
		/// </summary>
		/// <param name="field"><see cref="Fields"/> field to delete</param>
		public static void DeleteFieldFromConfig(Fields field)
		{
			Instance.FormFields.Remove(field);
			SaveConfig();
		}

		/// <summary>
		/// Fills a WordInterop TextField with text
		/// </summary>
		/// <param name="app">The Word Application containing the documents with FormFields to fill</param>
		/// <param name="field">The FormField to fill with Text</param>
		/// <param name="text">The Text to Fill</param>
		private static void FillFormField(Word.Application app, Word.FormField field, string text)
		{
			if (text == null)
				return;
			text = ReportUtils.TransformTextToWord(text);
			field.Select();
			for (int i = 1; i < 6; i++)
			{
				field.Range.Paragraphs.TabStops.Add(i * 14);
			}
			app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
			app.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);
			if (text.Length > 254)
			{
				field.Result = " ";
				app.Selection.Text = text.Substring(0, 200);
				field.Result = field.Result.TrimEnd() + " ";
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeText(text.Substring(200));
				//Remove first space before text
				field.Select();
				app.Selection.MoveLeft(Word.WdUnits.wdCharacter, 1);
				app.Selection.MoveRight(Word.WdUnits.wdCharacter, 1);
				app.Selection.TypeBackspace();
			}
			else
			{
				field.Result = text;
			}
		}
	}
}
