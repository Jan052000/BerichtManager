using Word = Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System.Globalization;
using BerichtManager.HelperClasses;
using BerichtManager.Extensions;
using BerichtManager.OwnControls;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using BerichtManager.WordTemplate.Exceptions;

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
		/// <see cref="Dictionary{TKey, TValue}"/> containing form field order and <see cref="Type"/>s
		/// Note that <see cref="Word.FormFields"/> start at index <c>1</c>
		/// </summary>
		private Dictionary<Fields, FormField> FormFields { get; set; } = GetInitialConfig();

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> to switch an <see cref="object"/> to a respective <see cref="Type"/>
		/// </summary>
		private Dictionary<Type, Func<string, object?>> TypeSwitchDict { get; } = new Dictionary<Type, Func<string, object?>>()
		{
			{typeof(int), (o) => int.Parse(o)},
			{typeof(int?), (o) => int.TryParse(o, out var val) ? val : null},
			{typeof(string), (o) => o},
			{typeof(DateTime), (o) => DateTime.ParseExact(o, "dd.MM.yyyy", new CultureInfo("de-DE"))},
			{typeof(DateTime?), (o) => DateTime.TryParseExact(o, DateTimeUtils.DATEFORMAT, MainForm.Culture, DateTimeStyles.None, out var val) ? val : null},
			{typeof(bool), (o) => bool.Parse(o)},
			{typeof(bool?), (o) => bool.TryParse(o, out var val) ? val : null},
			{typeof(Null), (o) => null}
		};

		private class Null { }

		#region Singleton
		private static FormFieldHandler? Singleton { get; set; }
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
		internal static Dictionary<Fields, FormField> GetInitialConfig()
		{
			Dictionary<Fields, FormField> dict = new Dictionary<Fields, FormField>();

			List<MemberInfo> members = typeof(Fields).GetMembers().Where(member => member.MemberType == MemberTypes.Field && Enum.IsDefined(typeof(Fields), member.Name)).ToList();

			List<Fields> fields = new List<Fields>();
			foreach (Fields field in Enum.GetValues(typeof(Fields)))
				fields.Add(field);

			for (int i = 0; i < fields.Count; i++)
			{
				string fieldName = Enum.GetName(typeof(Fields), fields[i])!;
				MemberInfo memberInfo = members.First(m => m.Name == fieldName);
				FieldsTypeAttribute? typeAttr = memberInfo.GetCustomAttribute<FieldsTypeAttribute>();
				FieldAttribute? fieldAttr = memberInfo.GetCustomAttribute<FieldAttribute>();
				if (typeAttr == null)
					throw new Exception($"{typeof(Fields).Name}.{fieldName} is missing a FieldsTypeAttribute!");
				dict.Add(fields[i], new FormField(i + 1, typeAttr.FieldType, fieldAttr != null ? fieldAttr.FieldFormattedName : fieldName));
			}

			return dict;
		}

		/// <summary>
		/// Generates a clone of the internal form field config
		/// </summary>
		/// <returns>Clone of internal form field config</returns>
		public static Dictionary<Fields, FormField> GetCurrentFields()
		{
			return Instance.FormFields.Where(_ => true).ToDictionary(kvp1 => kvp1.Key, kvp2 => kvp2.Value);
		}

		/// <summary>
		/// Gets the index of a <see cref="Word.FormField"/> in the template config
		/// </summary>
		/// <param name="field"><see cref="Fields"/> value of <see cref="Word.FormField"/> to get index of</param>
		/// <returns>Index of <paramref name="field"/> in the template config or <see langword="null"/> if <paramref name="field"/> was not found</returns>
		public static int? GetFormFieldIndex(Fields field)
		{
			if (!TryGetFormField(field, out FormField? info))
				return null;
			return info?.Index;
		}

		/// <summary>
		/// Tries to get the <see cref="FormField"/> object containing field index and type
		/// </summary>
		/// <param name="field"><see cref="Fields"/> field to get info of</param>
		/// <param name="info"><see cref="FormField"/> object containing field index and type or <see langword="null"/> if <paramref name="field"/> was not found</param>
		/// <returns><see langword="true"/> if <paramref name="field"/> is key of <see cref="FormFieldHandler"/></returns>
		public static bool TryGetFormField(Fields field, [NotNullWhen(true)] out FormField? info)
		{
			info = null;
			if (!Instance.FormFields.TryGetValue(field, out FormField? _info))
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
		/// <exception cref="UnknownFieldTypeException">Thrown if the <see cref="Type"/> of value for <paramref name="field"/> is not in <see cref="TypeSwitchDict"/></exception>
		/// <exception cref="NullReferenceException">Thrown if <see langword="null"/> would be assigned to a non nullable <see cref="Type"/></exception>
		public static T? GetValueFromDoc<T>(Fields field, Word.Document doc)
		{
			if (!TryGetFormField(field, out FormField? info))
			{
				try
				{
					return (T?)Instance.TypeSwitchDict[typeof(Null)]("");
				}
				catch (NullReferenceException)
				{
					throw new NullReferenceException($"Can not set value of type {typeof(T).Name} to null");
				}
			}
			if (typeof(T) == typeof(string))
				return (T?)Instance.TypeSwitchDict[typeof(string)](doc.FormFields[info.Index].Result);
			if (!Instance.TypeSwitchDict.TryGetValue(info.FieldType, out Func<string, object?>? convert))
				throw new UnknownFieldTypeException(info.FieldType);
			return (T?)convert(doc.FormFields[info.Index].Result);
		}

		/// <summary>
		/// Sets the value of the <paramref name="field"/> in <paramref name="doc"/> to be <paramref name="value"/>
		/// </summary>
		/// <param name="field"><see cref="Fields"/> field to set value</param>
		/// <param name="doc"><see cref="Word.Document"/> to set value of <paramref name="field"/> in</param>
		/// <param name="value">Value to set <paramref name="field"/> to</param>
		public static void SetValueInDoc(Fields field, Word.Document doc, string? value)
		{
			if (!TryGetFormField(field, out FormField? info))
				return;
			FillFormField(doc.Application, doc.FormFields[info!.Index], value);
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
			Dictionary<Fields, FormField>? _FormFields = JsonConvert.DeserializeObject<Dictionary<Fields, FormField>>(File.ReadAllText(FormFieldConfigPath));
			if (_FormFields != null && _FormFields!.KeyValuePairsEqualNoSequence(FormFields!) == true)
				FormFields = _FormFields!;
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
			if (Instance.FormFields.TryGetValue(field, out FormField? form))
			{
				if (form.Index == newIndex)
					return;
				form.Index = newIndex;
			}
			else
			{
				Dictionary<Fields, FormField> defaultConfig = GetInitialConfig();
				FormField defaultForm = defaultConfig[field]!;
				FormField newForm = new FormField(index: newIndex, type: defaultForm.FieldType, defaultForm.DisplayText);
				Instance.FormFields.Add(field, newForm);
			}
			SaveConfig();
		}

		/// <summary>
		/// Updates config to match <paramref name="fields"/>
		/// </summary>
		/// <param name="fields"><see cref="List{T}"/> of values to construct new config with</param>
		/// <exception cref="ArgumentException">Thrown if an index in <paramref name="fields"/> is invalid</exception>
		public static void UpdateFormFieldIndexes(List<(Fields Field, int Index)> fields)
		{
			Dictionary<Fields, FormField> newFormFields = new Dictionary<Fields, FormField>();
			Dictionary<Fields, FormField> initial = GetInitialConfig();
			foreach ((Fields Field, int Index) in fields)
			{
				if (Index < 1)
					throw new ArgumentException($"{Index} is an invalid index, Word form fields start at index 1", "newIndex");
				newFormFields.Add(Field, new FormField(Index, initial[Field].FieldType, initial[Field].DisplayText));
			}

			Instance.FormFields = newFormFields;
			SaveConfig();
		}

		/// <summary>
		/// Sorts <see cref="FormFields"/> and saves it at to <see cref="FormFieldConfigPath"/>
		/// </summary>
		private static void SaveConfig()
		{
			SortFormFields();
			if (Instance.FormFields!.KeyValuePairsEqualNoSequence(GetInitialConfig()!))
			{
				if (File.Exists(FormFieldConfigPath))
					File.Delete(FormFieldConfigPath);
				return;
			}
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
		private static void FillFormField(Word.Application app, Word.FormField field, string? text)
		{
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
