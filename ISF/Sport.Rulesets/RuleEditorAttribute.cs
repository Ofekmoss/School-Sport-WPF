using System;

namespace Sport.Rulesets
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=true)]
	public sealed class RuleEditorAttribute : Attribute
	{
		private string _typeName;
		public string EditorTypeName
		{
			get { return _typeName; }
		}

		public RuleEditorAttribute()
		{
			_typeName = string.Empty;
		}

		public RuleEditorAttribute(string typeName)
		{
			_typeName = typeName;
		}

		public RuleEditorAttribute(Type type)
		{
			_typeName = type.AssemblyQualifiedName;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			RuleEditorAttribute pea = obj as RuleEditorAttribute;

			if (pea != null)
				return pea.EditorTypeName == _typeName;

			return false;
		}


		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
	}
}
