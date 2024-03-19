using System;
using System.Windows.Forms;
using Sport.Rulesets;

namespace Sport.Producer.UI
{
	public abstract class RuleTypeEditor
	{
		public abstract Control Edit(Rule rule, RuleScope scope);
		public abstract void EndEdit();
	}
}
