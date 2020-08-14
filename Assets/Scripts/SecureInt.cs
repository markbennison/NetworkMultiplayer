using System;

public class SecureInt
{
	private int value;
	private int offset;
	Random randomGenerator = new Random();

	public SecureInt()
	{
		offset = randomGenerator.Next(-1000, 1000);
		this.value = offset;
	}

	public SecureInt(int value = 0)
	{
		offset = randomGenerator.Next(-1000, 1000);
		this.value = value + offset;
	}

	public int Get()
	{
		return value - offset;
	}

	public void Set(int value = 0)
	{
		offset = randomGenerator.Next(-1000, 1000);
		this.value = value + offset;
	}

	public void Dispose()
	{
		offset = 0;
		value = 0;
	}

	public void AddInt(int operand)
	{
		// Generate new offset
		int newOffset = randomGenerator.Next(-1000, 1000);
		// Perform int addition
		value = Get() + operand + newOffset;
		offset = newOffset;
	}

	public void SubtractInt(int operand)
	{
		// Generate new offset
		int newOffset = randomGenerator.Next(-1000, 1000);
		// Perform int subtraction
		value = Get() - operand + newOffset;
		offset = newOffset;
	}

	public override string ToString()
	{
		return Get().ToString();
	}

	public static SecureInt operator +(SecureInt value1, SecureInt value2)
	{
		return new SecureInt(value1.Get() + value2.Get());
	}

	public static SecureInt operator -(SecureInt value1, SecureInt value2)
	{
		return new SecureInt(value1.Get() - value2.Get());
	}

	public static SecureInt operator *(SecureInt value1, SecureInt value2)
	{
		return new SecureInt(value1.Get() * value2.Get());
	}

	public static SecureInt operator /(SecureInt value1, SecureInt value2)
	{
		return new SecureInt(value1.Get() / value2.Get());
	}
}
