using System.IO;
using System.Text.RegularExpressions;
using System;
namespace Utils.IniConfig
{

	public class IniSection
	{
		public static Regex RegexSectionDefine = new Regex("^\\[(\\w+)\\]");

		public static Regex RegexVariable = new Regex("^(\\w+)\\s*\\=\\s*(\\S+)");

		protected string _variableName;

		protected string _variableValue;

		private IniSection _prev;

		private IniSection _next;

		private IniSection _childHead;

		private IniSection _childTail;

		public string name => _variableName;

		public string value
		{
			get
			{
				return _variableValue;
			}
			set
			{
				_variableValue = value;
			}
		}

		public IniSection childHead => _childHead;

		public IniSection childTail => _childTail;

		public IniSection next => _next;

		public IniSection prev => _prev;

		public IniSection(ref IniSection head, ref IniSection tail, string name, string value)
		{
			_childHead = null;
			_childTail = null;
			if (name != null)
			{
				_variableName = name;
				if (value != null)
				{
					_variableValue = value;
				}
				if (head == null)
				{
					head = this;
					tail = this;
					_next = null;
					_prev = null;
				}
				else
				{
					tail._next = this;
					_prev = tail;
					_next = null;
					tail = this;
				}
			}
		}

		public IniSection addChild(string name, string value)
		{
			if (name != null)
			{
				return new IniSection(ref _childHead, ref _childTail, name, value);
			}
			return null;
		}

		public IniSection removeChild(IniSection child)
		{
			for (IniSection iniSection = _childHead; iniSection != null; iniSection = iniSection.next)
			{
				if (iniSection == child)
				{
					if (child._prev != null)
					{
						child._prev._next = child._next;
					}
					else
					{
						_childHead = child._next;
					}
					if (child._next != null)
					{
						child._next._prev = child._prev;
					}
					else
					{
						_childTail = child._prev;
					}
					child._next = null;
					child._prev = null;
					return child;
				}
			}
			return null;
		}

		public bool getIntValue(out int value)
		{
			return int.TryParse(_variableValue, out value);
		}

		public bool setIntValue(int newValue)
		{
			_variableValue = newValue.ToString();
			return true;
		}

		public bool getFloatValue(out float value)
		{
			return float.TryParse(_variableValue, out value);
		}

		public bool setFloatValue(float newValue)
		{
			_variableValue = newValue.ToString();
			return true;
		}

		public string readSection(StreamReader sr)
		{
			string text;
			while ((text = sr.ReadLine()) != null)
			{
				if (RegexSectionDefine.Match(text).Success)
				{
					return text;
				}
				Match match = RegexVariable.Match(text);
				if (match.Success)
				{
					addChild(match.Groups[1].Value, match.Groups[2].Value);
				}
			}
			return null;
		}

		public int writeSection(StreamWriter sw)
		{
			sw.WriteLine("[" + name + "]");
			for (IniSection iniSection = _childHead; iniSection != null; iniSection = iniSection.next)
			{
				sw.Write(iniSection.name);
				sw.Write(" = ");
				if (iniSection.value != null)
				{
					sw.Write(iniSection.value);
				}
				sw.WriteLine("");
			}
			sw.WriteLine("");
			return 0;
		}
	}
	public class IniFile : IDisposable
	{
		private bool _disposed;

		private bool _dirty;

		private IniSection _head;

		private IniSection _tail;

		private string _filename;

		public IniFile(string filename)
		{
			construct(filename);
		}

		~IniFile()
		{
			dispose(disposing: false);
		}

		public void Dispose()
		{
			dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public string getValue(string section, string variable, string defaultParam)
		{
			string result = defaultParam;
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				IniSection iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null)
				{
					result = iniSection2.value;
					if (result == null)
					{
						result = defaultParam;
					}
					return result;
				}
			}
			setValue(section, variable, defaultParam);
			return result;
		}

		public int setValue(string section, string variable, string newValue)
		{
			_dirty = true;
			IniSection iniSection = findSection(section);
			if (iniSection == null)
			{
				iniSection = _head.addChild(section, null);
			}
			IniSection iniSection2 = findVariable(iniSection, variable);
			if (iniSection2 == null)
			{
				iniSection.addChild(variable, newValue);
			}
			else
			{
				iniSection2.value = newValue;
			}
			return 1;
		}

		public int getValue(string section, string variable, int defaultParam)
		{
			return getIntValue(section, variable, defaultParam);
		}

		public bool setValue(string section, string variable, int newValue)
		{
			return setIntValue(section, variable, newValue);
		}

		public uint getValue(string section, string variable, uint defaultParam)
		{
			return (uint)getIntValue(section, variable, (int)defaultParam);
		}

		public bool setValue(string section, string variable, uint newValue)
		{
			return setIntValue(section, variable, (int)newValue);
		}

		public float getValue(string section, string variable, float defaultParam)
		{
			return getFloatValue(section, variable, defaultParam);
		}

		public bool setValue(string section, string variable, float newValue)
		{
			return setFloatValue(section, variable, newValue);
		}

		public bool getValue(string section, string variable, bool defaultParam)
		{
			return getIntValue(section, variable, defaultParam ? 1 : 0) != 0;
		}

		public bool setValue(string section, string variable, bool newValue)
		{
			return setIntValue(section, variable, newValue ? 1 : 0);
		}

		public int getIntValue(string section, string variable, int defaultParam)
		{
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				IniSection iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null && iniSection2.getIntValue(out var value))
				{
					return value;
				}
			}
			setIntValue(section, variable, defaultParam);
			return defaultParam;
		}

		public bool setIntValue(string section, string variable, int newValue)
		{
			_dirty = true;
			IniSection iniSection = findSection(section);
			if (iniSection == null)
			{
				iniSection = _head.addChild(section, null);
			}
			IniSection iniSection2 = findVariable(iniSection, variable);
			if (iniSection2 == null)
			{
				iniSection2 = iniSection.addChild(variable, null);
			}
			return iniSection2.setIntValue(newValue);
		}

		public float getFloatValue(string section, string variable, float defaultParam)
		{
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				IniSection iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null && iniSection2.getFloatValue(out var value))
				{
					return value;
				}
			}
			setFloatValue(section, variable, defaultParam);
			return defaultParam;
		}

		public bool setFloatValue(string section, string variable, float newValue)
		{
			_dirty = true;
			IniSection iniSection = findSection(section);
			if (iniSection == null)
			{
				iniSection = _head.addChild(section, null);
			}
			IniSection iniSection2 = findVariable(iniSection, variable);
			if (iniSection2 == null)
			{
				iniSection2 = iniSection.addChild(variable, null);
			}
			return iniSection2.setFloatValue(newValue);
		}

		public int deleteSection(string section)
		{
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				iniSection = _head.removeChild(iniSection);
			}
			if (iniSection != null)
			{
				_dirty = true;
				return 1;
			}
			return 0;
		}

		public int deleteVariable(string section, string variable)
		{
			IniSection iniSection = findSection(section);
			IniSection iniSection2 = null;
			if (iniSection != null)
			{
				iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null)
				{
					iniSection2 = iniSection.removeChild(iniSection2);
				}
			}
			if (iniSection2 != null)
			{
				_dirty = true;
				return 1;
			}
			return 0;
		}

		public IniSection findSection(string section)
		{
			if (section == null)
			{
				if (_head != null)
				{
					return _head.childHead;
				}
				return null;
			}
			if (_head != null)
			{
				for (IniSection iniSection = _head.childHead; iniSection != null; iniSection = iniSection.next)
				{
					if (iniSection.name == section)
					{
						return iniSection;
					}
				}
			}
			return null;
		}

		public IniSection findVariable(IniSection section, string variable)
		{
			if (section != null)
			{
				for (IniSection iniSection = section.childHead; iniSection != null; iniSection = iniSection.next)
				{
					if (iniSection.name == variable)
					{
						return iniSection;
					}
				}
			}
			return null;
		}

		public void clear()
		{
			_head = null;
			_tail = null;
			new IniSection(ref _head, ref _tail, "Root", null);
		}

		private void construct(string filename)
		{
			_dirty = false;
			_head = null;
			_tail = null;
			if (filename == null)
			{
				return;
			}
			_filename = filename;
			try
			{
				using StreamReader sr = new StreamReader(_filename);
				readFromFile(sr);
			}
			catch (Exception)
			{
				new IniSection(ref _head, ref _tail, "Root", null);
			}
		}

		private void dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (_dirty && !string.IsNullOrEmpty(_filename))
				{
					// 当文件被修改且路径有效时，执行保存
					using (var sw = new StreamWriter(_filename))
					{
						writeToFile(sw);
					}
					_dirty = false;
				}
				if (disposing)
				{
					_head = null;
					_tail = null;
					_filename = null;
				}
				_disposed = true;
			}
		}

		private bool readFromFile(StreamReader sr)
		{
			if (_head != null)
			{
				return false;
			}
			new IniSection(ref _head, ref _tail, "Root", null);
			string text = null;
			while (true)
			{
				if (text == null)
				{
					text = sr.ReadLine();
				}
				if (text == null)
				{
					break;
				}
				Match match = IniSection.RegexSectionDefine.Match(text);
				text = ((!match.Success) ? null : _head.addChild(match.Groups[1].Value, null)?.readSection(sr));
			}
			return true;
		}

		private bool writeToFile(StreamWriter sw)
		{
			for (IniSection iniSection = _head.childHead; iniSection != null; iniSection = iniSection.next)
			{
				iniSection.writeSection(sw);
			}
			return true;
		}
	}
}

