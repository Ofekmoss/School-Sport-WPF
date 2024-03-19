using System;

namespace Sport.Common
{
	public enum EquationOperator
	{
		Add,
		Sub,
		Mul,
		Div
	}

	public class EquationVariables
	{
		private System.Collections.Hashtable variables;

		public EquationVariables()
		{
			variables = new System.Collections.Hashtable();
		}

		public double this[string variable]
		{
			get { return (double) variables[variable]; }
			set { variables[variable] = value; }
		}

		public bool Contains(string variable)
		{
			return variables.ContainsKey(variable);
		}

		public void Remove(string variable)
		{
			variables.Remove(variable);
		}

		public void Add(string variable, double value)
		{
			variables.Add(variable, value);
		}
	}

	public abstract class Equation
	{
		public Equation()
		{
		}

		public abstract double Resolve(EquationVariables variables);

		public double Resolve()
		{
			return Resolve(null);
		}

		private static int Find(string equation, char op, int start, int length)
		{
			int brac = 0;
			for (int n = start; n < start + length; n++)
			{
				if (brac > 0)
				{
					if (equation[n] == ')')
						brac--;
				}
				else
				{
					if (equation[n] == '(')
						brac++;
					else if (equation[n] == op)
						return n;
				}
			}

			return -1;
		}

		private struct Operator
		{
			public char				Character;
			public EquationOperator Operation;
			public Operator(char c, EquationOperator o)
			{
				Character = c;
				Operation = o;
			}
		}

		private static Operator[] operators = new Operator[]
			{
				new Operator('+', EquationOperator.Add),
				new Operator('-', EquationOperator.Sub),
				new Operator('*', EquationOperator.Mul),
				new Operator('/', EquationOperator.Div)
			};


		private static Equation Evaluate(string equation, int start, int length)
		{
			// Removing white spaces
			while (length > 0 && Char.IsWhiteSpace(equation[start + length - 1]))
				length--;
			while (length > 0 && Char.IsWhiteSpace(equation[start]))
			{
				start++;
				length--;
			}

			// Open bracets
			if (equation[start] == '(' &&
				equation[start + length - 1] == ')')
				return Evaluate(equation, start + 1, length - 2);

			int pos;

			for (int o = 0; o < operators.Length; o++)
			{
				pos = Find(equation, operators[o].Character, start, length);

				if (pos >= 0)
				{
					return new OperatorEquation(operators[o].Operation,
						Evaluate(equation, start, pos - start),
						Evaluate(equation, pos + 1, length - (pos - start + 1)));
				}
			}

			string s = equation.Substring(start, length);
			try
			{
				double val = Double.Parse(s);

				return new ValueEquation(val);
			}
			catch
			{
				return new VariableEquation(s);
			}
		}

		public static Equation Evaluate(string equation)
		{
			return Evaluate(equation, 0, equation.Length);
		}
	}

	public class ValueEquation : Equation
	{
		private double _value;
		public double Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ValueEquation(double value)
		{
			_value = value;
		}

		public override double Resolve(EquationVariables variables)
		{
			return _value;
		}
	}

	public class VariableEquation : Equation
	{
		private string _variable;
		public string Variable
		{
			get { return _variable; }
			set { _variable = value; }
		}

		public VariableEquation(string variable)
		{
			_variable = variable;
		}

		public override double Resolve(EquationVariables variables)
		{
			if (variables == null || !variables.Contains(_variable))
				throw new EquationException("Variable not found");

			return variables[_variable];
		}
	}

	public class OperatorEquation : Equation
	{
		private EquationOperator _operator;
		public EquationOperator Operator
		{
			get { return _operator; }
			set { _operator = value; }
		}

		private Equation _left;
		public Equation Left
		{
			get { return _left; }
			set { _left = value; }
		}

		private Equation _right;
		public Equation Right
		{
			get { return _right; }
			set { _right = value; }
		}
        
		public OperatorEquation(EquationOperator op, Equation left, Equation right)
		{
			_operator = op;
			_left = left;
			_right = right;
		}

		public override double Resolve(EquationVariables variables)
		{
			switch (_operator)
			{
				case (EquationOperator.Add):
					return _left.Resolve(variables) + _right.Resolve(variables);
				case (EquationOperator.Sub):
					return _left.Resolve(variables) - _right.Resolve(variables);
				case (EquationOperator.Mul):
					return _left.Resolve(variables) * _right.Resolve(variables);
				case (EquationOperator.Div):
					return _left.Resolve(variables) / _right.Resolve(variables);
			}

			throw new EquationException("Undefined operator");
		}
	}

	public class EquationException : Exception
	{
		public EquationException(string message)
			: base(message)
		{
		}
	}
}
