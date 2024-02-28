using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.Networking;

public class PasswordGeneratorSettings
{
	const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
	const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	const string NUMERIC_CHARACTERS = "0123456789";
	const string SPECIAL_CHARACTERS = @"!#$%&*@\";
	const int PASSWORD_LENGTH_MIN = 8;
	const int PASSWORD_LENGTH_MAX = 128;

	public bool IncludeLowercase { get; set; }
	public bool IncludeUppercase { get; set; }
	public bool IncludeNumbers { get; set; }
	public bool IncludeSpecial { get; set; }
	public int PasswordLength { get; set; }
	public string CharacterSet { get; set; }
	public int MaximumAttempts { get; set; }

	public PasswordGeneratorSettings(bool includeLowercase, bool includeUppercase, bool includeNumbers, bool includeSpecial, int passwordLength)
	{
		IncludeLowercase = includeLowercase;
		IncludeUppercase = includeUppercase;
		IncludeNumbers = includeNumbers;
		IncludeSpecial = includeSpecial;
		PasswordLength = passwordLength;

		StringBuilder characterSet = new StringBuilder();

		if (includeLowercase)
		{
			characterSet.Append(LOWERCASE_CHARACTERS);
		}

		if (includeUppercase)
		{
			characterSet.Append(UPPERCASE_CHARACTERS);
		}

		if (includeNumbers)
		{
			characterSet.Append(NUMERIC_CHARACTERS);
		}

		if (includeSpecial)
		{
			characterSet.Append(SPECIAL_CHARACTERS);
		}

		CharacterSet = characterSet.ToString();
	}

	public bool IsValidLength()
	{
		return PasswordLength >= PASSWORD_LENGTH_MIN && PasswordLength <= PASSWORD_LENGTH_MAX;
	}

	public string LengthErrorMessage()
	{
		return string.Format("Password length must be between {0} and {1} characters", PASSWORD_LENGTH_MIN, PASSWORD_LENGTH_MAX);
	}
}


public static class PasswordGenerator
{

	/// <summary>
	/// Generates a random password based on the rules passed in the settings parameter
	/// </summary>
	/// <param name="settings">Password generator settings object</param>
	/// <returns>Password or try again</returns>
	public static string GeneratePassword(PasswordGeneratorSettings settings)
	{
		const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
		char[] password = new char[settings.PasswordLength];
		int characterSetLength = settings.CharacterSet.Length;

		System.Random random = new System.Random();
		for (int characterPosition = 0; characterPosition < settings.PasswordLength; characterPosition++)
		{
			password[characterPosition] = settings.CharacterSet[random.Next(characterSetLength - 1)];

			bool moreThanTwoIdenticalInARow =
				characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS
				&& password[characterPosition] == password[characterPosition - 1]
				&& password[characterPosition - 1] == password[characterPosition - 2];

			if (moreThanTwoIdenticalInARow)
			{
				characterPosition--;
			}
		}

		return string.Join(null, password);
	}


	/// <summary>
	/// When you give it a password and some settings, it validates the password against the settings.
	/// </summary>
	/// <param name="settings">Password settings</param>
	/// <param name="password">Password to test</param>
	/// <returns>True or False to say if the password is valid or not</returns>
	public static bool PasswordIsValid(PasswordGeneratorSettings settings, string password)
	{
		const string REGEX_LOWERCASE = @"[a-z]";
		const string REGEX_UPPERCASE = @"[A-Z]";
		const string REGEX_NUMERIC = @"[\d]";
		const string REGEX_SPECIAL = @"([!#$%&*@\\])+";

		bool lowerCaseIsValid = !settings.IncludeLowercase || (settings.IncludeLowercase && Regex.IsMatch(password, REGEX_LOWERCASE));
		bool upperCaseIsValid = !settings.IncludeUppercase || (settings.IncludeUppercase && Regex.IsMatch(password, REGEX_UPPERCASE));
		bool numericIsValid = !settings.IncludeNumbers || (settings.IncludeNumbers && Regex.IsMatch(password, REGEX_NUMERIC));
		bool symbolsAreValid = !settings.IncludeSpecial || (settings.IncludeSpecial && Regex.IsMatch(password, REGEX_SPECIAL));

		return lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid;
	}

	public static string MD5Hash(string input)
	{
		StringBuilder hash = new StringBuilder();
		MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
		byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

		for (int i = 0; i < bytes.Length; i++)
		{
			hash.Append(bytes[i].ToString("x2"));
		}
		return hash.ToString();
	}


	public static string SHA512(string input)
	{
		StringBuilder hash = new StringBuilder();
		SHA512 shaM = new SHA512Managed();
		byte[] bytes = shaM.ComputeHash(new UTF8Encoding().GetBytes(input));

		for (int i = 0; i < bytes.Length; i++)
		{
			hash.Append(bytes[i].ToString("x2"));
		}
		return hash.ToString();
	}

	public static string url_encode(string input)
	{
		return UnityWebRequest.EscapeURL(input);
	}

	public static string Base64(string input)
	{
		byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
		return Convert.ToBase64String(bytesToEncode);
	}

	public static string SHA1(string input)
	{
		StringBuilder hash = new StringBuilder();
		SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
		byte[] bytes = sha1.ComputeHash(new UTF8Encoding().GetBytes(input));

		for (int i = 0; i < bytes.Length; i++)
		{
			hash.Append(bytes[i].ToString("x2"));
		}
		return hash.ToString();
	}

	public static string SHA256(string input)
	{
		StringBuilder hash = new StringBuilder();
		SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
		byte[] bytes = sha256.ComputeHash(new UTF8Encoding().GetBytes(input));

		for (int i = 0; i < bytes.Length; i++)
		{
			hash.Append(bytes[i].ToString("x2"));
		}
		return hash.ToString();
	}
}