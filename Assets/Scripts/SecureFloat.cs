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

	public void Dispose()
	{
		offset = 0;
		value = 0;
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
