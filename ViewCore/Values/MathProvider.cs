﻿using System;
using System.Collections;
using System.Collections.Generic;
using ViewObjects;

namespace ViewTo.Values
{
	public abstract class MathProvider<T>
	{
		public abstract T Divide(T a, T b);
		public abstract T Multiply(T a, T b);
		public abstract T Add(T a, T b);
		public abstract T Negate(T a);

		public virtual T Subtract(T a, T b) => Add(a, Negate(b));

		public virtual T Normalize(T value, T max, T min) => Divide(Subtract(value, min), Subtract(max, min));

		public virtual IEnumerable<T> Normalize(List<T> inputValues, T max, T min)
		{
			if (!inputValues.Valid()) return Array.Empty<T>();

			var values = new T[inputValues.Count];

			for (var i = 0; i < values.Length; i++)
				values[i] = Normalize(inputValues[i], max, min);

			return values;
		}

		public virtual IEnumerable<T> Normalize(List<T> inputValues, List<T> maxValues, T min)
		{
			if (!inputValues.Valid() || !maxValues.Valid() || inputValues.Count != maxValues.Count)
				return Array.Empty<T>();

			var values = new T[inputValues.Count];

			for (var i = 0; i < values.Length; i++)
			{
				values[i] = Normalize(inputValues[i], maxValues[i], min);
			}

			return values;
		}

		public virtual IEnumerable<T> Normalize(List<T> inputValues, List<T> maxValues, List<T> minValues)
		{
			if (!inputValues.Valid()
			    || !maxValues.Valid()
			    || !minValues.Valid()
			    || inputValues.Count != maxValues.Count
			    || inputValues.Count != minValues.Count
			   )
				return Array.Empty<T>();

			var values = new T[inputValues.Count];

			for (var i = 0; i < values.Length; i++)
				values[i] = Normalize(inputValues[i], maxValues[i], minValues[i]);

			return values;
		}

	}

	public class DoubleMath : MathProvider<double>
	{
		public override double Divide(double a, double b) => a / b;

		public override double Multiply(double a, double b) => a * b;

		public override double Add(double a, double b) => a + b;

		public override double Negate(double a) => -a;
	}

	public class UintMath : MathProvider<uint>
	{
		public override uint Divide(uint a, uint b) => a / b;

		public override uint Multiply(uint a, uint b) => a * b;

		public override uint Add(uint a, uint b) => a + b;

		public override uint Negate(uint a) => 0;
	}

	public class IntMath : MathProvider<int>
	{
		public override int Divide(int a, int b) => a / b;

		public override int Multiply(int a, int b) => a * b;

		public override int Add(int a, int b) => a + b;

		public override int Negate(int a) => -a;
	}

	// https://stackoverflow.com/questions/63694/creating-a-math-library-using-generics-in-c-sharp
	public class Fraction<T>
	{
		static MathProvider<T> _math;

		static Fraction()
		{
			if (typeof(T) == typeof(double))
				_math = new DoubleMath() as MathProvider<T>;
			else if (typeof(T) == typeof(int))
				_math = new IntMath() as MathProvider<T>;
			else if (typeof(T) == typeof(uint))
				_math = new UintMath() as MathProvider<T>;

			if (_math == null)
				throw new InvalidOperationException(
					"Type " + typeof(T).ToString() + " is not supported by Fraction.");
		}

		public T Numerator { get; private set; }

		public T Denominator { get; private set; }

		public Fraction(T numerator, T denominator)
		{
			Numerator = numerator;
			Denominator = denominator;
		}

		public static Fraction<T> operator +(Fraction<T> a, Fraction<T> b)
		{
			return new Fraction<T>(
				_math.Add(
					_math.Multiply(a.Numerator, b.Denominator),
					_math.Multiply(b.Numerator, a.Denominator)),
				_math.Multiply(a.Denominator, b.Denominator));
		}

		public static Fraction<T> operator -(Fraction<T> a, Fraction<T> b)
		{
			return new Fraction<T>(
				_math.Subtract(
					_math.Multiply(a.Numerator, b.Denominator),
					_math.Multiply(b.Numerator, a.Denominator)),
				_math.Multiply(a.Denominator, b.Denominator));
		}

		public static Fraction<T> operator /(Fraction<T> a, Fraction<T> b)
		{
			return new Fraction<T>(
				_math.Multiply(a.Numerator, b.Denominator),
				_math.Multiply(a.Denominator, b.Numerator));
		}

		// ... other operators would follow.
	}

}