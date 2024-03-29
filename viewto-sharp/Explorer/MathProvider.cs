﻿using System;

namespace ViewTo.Values
{

  public abstract class MathProvider<T> where T : IComparable<T>
  {
    public abstract T Divide(T a, T b);
    public abstract T Multiply(T a, T b);
    public abstract T Add(T a, T b);
    public abstract T Negate(T a);

    public T Clamp(T val, T min, T max)
    {
      if(val.CompareTo(min)<0) return min;
      return val.CompareTo(max)>0 ? max : val;
    }

    public T Subtract(T a, T b)
    {
      return Add(a, Negate(b));
    }

    // public virtual double Normalize(T value, T max, T min) => (double)Divide(Subtract(value, min), Subtract(max, min));
  }

  public class DoubleMath : MathProvider<double>
  {
    public override double Divide(double a, double b)
    {
      return!double.IsNaN(b) ? a/b : double.NaN;
    }

    public override double Multiply(double a, double b)
    {
      return a*b;
    }

    public override double Add(double a, double b)
    {
      return a+b;
    }

    public override double Negate(double a)
    {
      return-a;
    }
  }

  public class UintMath : MathProvider<uint>
  {
    public override uint Divide(uint a, uint b)
    {
      return b != 0 ? a/b : 0;
    }

    public override uint Multiply(uint a, uint b)
    {
      return a*b;
    }

    public override uint Add(uint a, uint b)
    {
      return a+b;
    }

    public override uint Negate(uint a)
    {
      return 0;
    }
  }

  public class IntMath : MathProvider<int>
  {
    public override int Divide(int a, int b)
    {
      return b != 0 ? a/b : 0;
    }

    public override int Multiply(int a, int b)
    {
      return a*b;
    }

    public override int Add(int a, int b)
    {
      return a+b;
    }

    public override int Negate(int a)
    {
      return-a;
    }
  }

  // https://stackoverflow.com/questions/63694/creating-a-math-library-using-generics-in-c-sharp

  public class Fraction<T> where T : IComparable<T>
  {
    private static MathProvider<T> _math;

    static Fraction()
    {
      if(typeof(T) == typeof(double))
      {
        _math = new DoubleMath() as MathProvider<T>;
      }
      else if(typeof(T) == typeof(int))
      {
        _math = new IntMath() as MathProvider<T>;
      }
      else if(typeof(T) == typeof(uint))
      {
        _math = new UintMath() as MathProvider<T>;
      }

      if(_math == null)
      {
        throw new InvalidOperationException(
          "Type "+typeof(T)+" is not supported by Fraction.");
      }
    }

    public Fraction(T numerator, T denominator)
    {
      Numerator = numerator;
      Denominator = denominator;
    }

    public T Numerator {get;}

    public T Denominator {get;}

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

  public abstract class ValueRangeProvider<T>
  {
    public T min;
    public T max;

    public abstract T span {get;}

    public override string ToString() => $"min:{min} max:{max} span:{span}";
  }

  public class FloatRange : ValueRangeProvider<float>
  {
    public override float span => Math.Abs(max-min);
  }

  public class DoubleRange : ValueRangeProvider<double>
  {

    public override double span => Math.Abs(max-min);

  }

  public class IntRange : ValueRangeProvider<int>
  {
    public override int span => Math.Abs(max-min);
  }

}
