using System;
using NUnit.Framework;

[TestFixture]
public class ObjectTests
{
	public interface IObjectTest
	{
		public string VirtualInterfaceMethod();

		public string InterfaceMethod();

	}

	public class BaseObject : IObjectTest
	{

		public virtual string VirtualClassMethod()
		{
			return$"{nameof(VirtualClassMethod)} executed! -- Called from Base Object";
		}

		public string ClassMethod()
		{
			return$"{nameof(ClassMethod)} executed! -- Called from Base Object";
		}

		public virtual string VirtualInterfaceMethod()
		{
			return$"{nameof(VirtualInterfaceMethod)} executed! -- Called from Base Object";
		}

		public string InterfaceMethod()
		{
			return$"{nameof(InterfaceMethod)} executed! -- Called from Base Object";
		}

	}

	public class DerivedObject : BaseObject
	{

		public new string ClassMethod()
		{
			return$"New {nameof(ClassMethod)} executed! -- Called from Derived Object";
		}

		public new string InterfaceMethod()
		{
			return$"New {nameof(InterfaceMethod)} executed! -- Called from Derived Object";
		}

		public override string VirtualClassMethod()
		{
			return$"{nameof(VirtualClassMethod)} executed! -- Called from Derived Object";
		}

		public override string VirtualInterfaceMethod()
		{
			return$"{nameof(VirtualInterfaceMethod)} executed! -- Called from Derived Object";
		}
	}

	[Test]
	public void ObjectInheritance()
	{
		
		BaseObject base_object = new BaseObject();
		DerivedObject derived_object = new DerivedObject();
		BaseObject casted_base_object = new DerivedObject();

		// base object
		Console.WriteLine($"From Base Object -- {base_object.InterfaceMethod()}");
		Console.WriteLine($"From Base Object -- {base_object.VirtualInterfaceMethod()}");
		Console.WriteLine($"From Base Object -- {base_object.ClassMethod()}");
		Console.WriteLine($"From Base Object -- {base_object.VirtualClassMethod()}");

		// derived object
		Console.WriteLine($"From Derived Object -- {derived_object.InterfaceMethod()}");
		Console.WriteLine($"From Derived Object -- {derived_object.VirtualInterfaceMethod()}");
		Console.WriteLine($"From Derived Object -- {derived_object.ClassMethod()}");
		Console.WriteLine($"From Derived Object -- {derived_object.VirtualClassMethod()}");

		// casted object
		Console.WriteLine($"From Casted Base Object -- {casted_base_object.InterfaceMethod()}");
		Console.WriteLine($"From Casted Base Object -- {casted_base_object.VirtualInterfaceMethod()}");
		Console.WriteLine($"From Casted Base Object -- {casted_base_object.ClassMethod()}");
		Console.WriteLine($"From Casted Base Object -- {casted_base_object.VirtualClassMethod()}");
		
		
		IObjectTest interface_base_object = new BaseObject();
		IObjectTest interface_derived_object = new DerivedObject();

		// base object
		Console.WriteLine($"From Interface Base Object -- {interface_base_object.InterfaceMethod()}");
		Console.WriteLine($"From Interface Base Object -- {interface_base_object.VirtualInterfaceMethod()}");

		// derived object
		Console.WriteLine($"From Interface Derived Object -- {interface_derived_object.InterfaceMethod()}");
		Console.WriteLine($"From Interface Derived Object -- {interface_derived_object.VirtualInterfaceMethod()}");

	}
}