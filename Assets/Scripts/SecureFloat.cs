using System;

public class SecureFloat
{
	private float value;
	private float offset;
	Random randomGenerator = new Random();

	public SecureFloat()
	{
		offset = randomGenerator.Next(-1000, 1000);
		this.value = offset;
	}

	public SecureFloat(float value = 0)
	{
		offset = randomGenerator.Next(-1000, 1000);
		this.value = value + offset;
	}

	public float Get()
	{
		return value - offset;
	}

	public void Set(float value = 0)
	{
		offset = randomGenerator.Next(-1000, 1000);
		this.value = value + offset;
	}

	public void Dispose()
	{
		offset = 0;
		value = 0;
	}

	public void AddFloat(float operand)
	{
		// Generate new offset
		float newOffset = randomGenerator.Next(-1000, 1000);
		// Perform float addition
		value = Get() + operand + newOffset;
		offset = newOffset;
	}

	public void SubtractFloat(float operand)
	{
		// Generate new offset
		float newOffset = randomGenerator.Next(-1000, 1000);
		// Perform float subtraction
		value = Get() - operand + newOffset;
		offset = newOffset;
	}

	public override string ToString()
	{
		return Get().ToString();
	}

	public static SecureFloat operator +(SecureFloat value1, SecureFloat value2)
	{
		return new SecureFloat(value1.Get() + value2.Get());
	}

	public static SecureFloat operator -(SecureFloat value1, SecureFloat value2)
	{
		return new SecureFloat(value1.Get() - value2.Get());
	}

	public static SecureFloat operator *(SecureFloat value1, SecureFloat value2)
	{
		return new SecureFloat(value1.Get() * value2.Get());
	}

	public static SecureFloat operator /(SecureFloat value1, SecureFloat value2)
	{
		return new SecureFloat(value1.Get() / value2.Get());
	}
}
