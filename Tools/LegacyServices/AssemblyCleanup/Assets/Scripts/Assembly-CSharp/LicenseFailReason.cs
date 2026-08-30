public enum LicenseFailReason
{
	iOS_Honeypot = 100,
	iOS_BinaryEncryption = 101,
	iOS_UserId = 102,
	iOS_CodeSignature = 103,
	iOS_FileManager = 104,
	iOS_JB = 105,
	Android_GooglePlayLicense = 200,
	Android_SigningCertificate = 201,
	Android_InstallerId = 202,
	Android_Debug = 203,
	Android_Emulator = 204,
	Android_UnauthorizedApps = 205,
	LicenseCheckerCorrupted = 206,
	UndefinedPlatform = 207,
	Unknown = 208
}
