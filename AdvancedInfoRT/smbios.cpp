#include "pch.h"
#include "smbios.h"

using namespace SMBIOSRT;

static const char* LocateStringA(const char* str, UINT i)
{
	static const char strNull[] = "";

	if (0 == i || 0 == *str) return strNull;

	while (--i)
	{
		str += strlen((char*)str) + 1;
	}
	return str;
}

static const char* toPointString(void* p)
{
	return (char*)p + ((PSMBIOSHEADER)p)->Length;
}

bool SMBIOS::ProcBIOSInfo(SMBIOS* T, void* p)
{
	PBIOSInfo pBIOS = (PBIOSInfo)p;
	const char* str = toPointString(p);
	const char* Vendor = LocateStringA(str, pBIOS->Vendor);
	const char* Version = LocateStringA(str, pBIOS->Version);
	const char* Date = LocateStringA(str, pBIOS->ReleaseDate);
	const int nVendor = (int)strlen(Vendor);
	const int nVersion = (int)strlen(Version);
	const int nDate = (int)strlen(Date);

	T->m_wszBIOSVendor = new WCHAR[nVendor + 1];
	T->m_wszBIOSVersion = new WCHAR[nVersion + 1];
	T->m_wszBIOSReleaseDate = new WCHAR[nDate + 1];
	if (T->m_wszBIOSVendor)
	{
		::SecureZeroMemory(T->m_wszBIOSVendor, sizeof(WCHAR) * (nVendor + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Vendor, nVendor, T->m_wszBIOSVendor, nVendor + 1);
	}
	if (T->m_wszBIOSVersion)
	{
		::SecureZeroMemory(T->m_wszBIOSVersion, sizeof(WCHAR) * (nVersion + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Version, nVersion, T->m_wszBIOSVersion, nVersion + 1);
	}
	if (T->m_wszBIOSReleaseDate)
	{
		::SecureZeroMemory(T->m_wszBIOSReleaseDate, sizeof(WCHAR) * (nDate + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Date, nDate, T->m_wszBIOSReleaseDate, nDate + 1);
	}
	if (pBIOS->Header.Length > 0x14)
	{
		T->m_BIOSSysVersion = pBIOS->MajorRelease << 16 | pBIOS->MinorRelease;
		T->m_BIOSECVersion = pBIOS->ECFirmwareMajor << 16 | pBIOS->ECFirmwareMinor;
	}
	return true;
}

bool SMBIOS::ProcSysInfo(SMBIOS * T, void* p)
{
	PSystemInfo pSystem = (PSystemInfo)p;
	const char* str = toPointString(p);
	const char* Manufactor = LocateStringA(str, pSystem->Manufacturer);
	const char* ProductName = LocateStringA(str, pSystem->ProductName);
	const char* Version = LocateStringA(str, pSystem->Version);
	const char* SerialNumber = LocateStringA(str, pSystem->SN);
	const int nManufactor = (int)strlen(Manufactor);
	const int nProductName = (int)strlen(ProductName);
	const int nVersion = (int)strlen(Version);
	const int nSerialNumber = (int)strlen(SerialNumber);

	T->m_wszSysManufactor = new WCHAR[nManufactor + 1];
	T->m_wszSysProductName = new WCHAR[nProductName + 1];
	T->m_wszSysVersion = new WCHAR[nVersion + 1];
	T->m_wszSysSerialNumber = new WCHAR[nSerialNumber];

	if (T->m_wszSysManufactor)
	{
		::SecureZeroMemory(T->m_wszSysManufactor, sizeof(WCHAR) * (nManufactor + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Manufactor, nManufactor, T->m_wszSysManufactor, nManufactor + 1);
	}
	if (T->m_wszSysProductName)
	{
		::SecureZeroMemory(T->m_wszSysProductName, sizeof(WCHAR) * (nProductName + 1));
		::MultiByteToWideChar(CP_ACP, NULL, ProductName, nProductName, T->m_wszSysProductName, nProductName + 1);
	}
	if (T->m_wszSysVersion)
	{
		::SecureZeroMemory(T->m_wszSysVersion, sizeof(WCHAR) * (nVersion + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Version, nVersion, T->m_wszSysVersion, nVersion + 1);
	}
	if (T->m_wszSysSerialNumber)
	{
		::SecureZeroMemory(T->m_wszSysSerialNumber, sizeof(WCHAR) * (nSerialNumber + 1));
		::MultiByteToWideChar(CP_ACP, NULL, SerialNumber, nSerialNumber, T->m_wszSysSerialNumber, nSerialNumber + 1);
	}
	if (pSystem->Header.Length > 0x08)
	{
		memcpy_s(&(T->m_SysUUID), sizeof(UUID), pSystem->UUID, 16);
	}
	if (pSystem->Header.Length > 0x19)
	{
		const char* SKU = LocateStringA(str, pSystem->SKUNumber);
		const char* Family = LocateStringA(str, pSystem->Family);
		const int nSKU = (int)strlen(SKU);
		const int nFamily = (int)strlen(Family);

		T->m_wszSysSKU = new WCHAR[nSKU + 1];
		T->m_wszSysFamily = new WCHAR[nFamily + 1];
		if (T->m_wszSysSKU)
		{
			::SecureZeroMemory(T->m_wszSysSKU, sizeof(WCHAR) * (nSKU + 1));
			::MultiByteToWideChar(CP_ACP, NULL, SKU, nSKU, T->m_wszSysSKU, nSKU + 1);
		}
		if (T->m_wszSysFamily)
		{
			::SecureZeroMemory(T->m_wszSysFamily, sizeof(WCHAR) * (nFamily + 1));
			::MultiByteToWideChar(CP_ACP, NULL, Family, nFamily, T->m_wszSysFamily, nFamily + 1);
		}
	}
	return true;
}

bool SMBIOS::ProcBoardInfo(SMBIOS * T, void* p)
{
	PBoardInfo pBoard = (PBoardInfo)p;
	const char* str = toPointString(p);
	const char* Manufactor = LocateStringA(str, pBoard->Manufacturer);
	const char* ProductName = LocateStringA(str, pBoard->Product);
	const char* Version = LocateStringA(str, pBoard->Version);
	const char* SerialNumber = LocateStringA(str, pBoard->SN);
	const char* AssetTag = LocateStringA(str, pBoard->AssetTag);
	const int nManufactor = (int)strlen(Manufactor);
	const int nProductName = (int)strlen(ProductName);
	const int nVersion = (int)strlen(Version);
	const int nSerialNumber = (int)strlen(SerialNumber);
	const int nAssetTag = (int)strlen(AssetTag);

	T->m_wszBoardManufactor = new WCHAR[nManufactor + 1];
	T->m_wszBoardProductName = new WCHAR[nProductName + 1];
	T->m_wszBoardVersion = new WCHAR[nVersion + 1];
	T->m_wszBoardSerialNumber = new WCHAR[nSerialNumber + 1];
	T->m_wszBoardAssetTag = new WCHAR[nAssetTag + 1];

	if (T->m_wszBoardManufactor)
	{
		::SecureZeroMemory(T->m_wszBoardManufactor, sizeof(WCHAR) * (nManufactor + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Manufactor, nManufactor, T->m_wszBoardManufactor, nManufactor + 1);
	}
	if (T->m_wszBoardProductName)
	{
		::SecureZeroMemory(T->m_wszBoardProductName, sizeof(WCHAR) * (nProductName + 1));
		::MultiByteToWideChar(CP_ACP, NULL, ProductName, nProductName, T->m_wszBoardProductName, nProductName + 1);
	}
	if (T->m_wszBoardVersion)
	{
		::SecureZeroMemory(T->m_wszBoardVersion, sizeof(WCHAR) * (nVersion + 1));
		::MultiByteToWideChar(CP_ACP, NULL, Version, nVersion, T->m_wszBoardVersion, nVersion + 1);
	}
	if (T->m_wszBoardSerialNumber)
	{
		::SecureZeroMemory(T->m_wszBoardSerialNumber, sizeof(WCHAR) * (nSerialNumber + 1));
		::MultiByteToWideChar(CP_ACP, NULL, SerialNumber, nSerialNumber, T->m_wszBoardSerialNumber, nSerialNumber + 1);
	}
	if (T->m_wszBoardAssetTag)
	{
		::SecureZeroMemory(T->m_wszBoardAssetTag, sizeof(WCHAR) * (nAssetTag + 1));
		::MultiByteToWideChar(CP_ACP, NULL, AssetTag, nAssetTag, T->m_wszBoardAssetTag, nAssetTag + 1);
	}
	if (pBoard->Header.Length > 0x08)
	{
		const char* Location = LocateStringA(str, pBoard->LocationInChassis);
		const size_t nLocation = strlen(Location);
		T->m_wszBoardLocation = new WCHAR[nLocation + 1];
		if (T->m_wszBoardAssetTag)
		{
			::SecureZeroMemory(T->m_wszBoardAssetTag, sizeof(WCHAR) * (nAssetTag + 1));
			::MultiByteToWideChar(CP_ACP, NULL, AssetTag, nAssetTag, T->m_wszBoardAssetTag, nAssetTag + 1);
		}
	}
	return true;
}


bool SMBIOS::DispatchStructType(void* pHdr)
{
	typedef struct {
		BYTE t;
		bool(*Proc)(SMBIOS* T, void* p);
	} TPFUNC;

	const TPFUNC	tpfunc[] = {
		{ 0, ProcBIOSInfo },
		{ 1, ProcSysInfo },
		{ 2, ProcBoardInfo },
	};

	PSMBIOSHEADER hdr = (PSMBIOSHEADER)pHdr;

	for (UINT i = 0; i < sizeof(tpfunc) / sizeof(TPFUNC); i++)
	{
		if (tpfunc[i].t == hdr->Type)
		{
			tpfunc[i].Proc(this, (void*)hdr);
			return true;
		}
	}
	return false;
}

void SMBIOS::ParseSMBIOSStruct(void* Addr, UINT Len)
{
	LPBYTE p = (LPBYTE)(Addr);
	const LPBYTE lastAddress = p + Len;
	PSMBIOSHEADER pHeader;

	for (;;) {
		pHeader = (PSMBIOSHEADER)p;
		if (pHeader->Type == 127 && pHeader->Length == 4)
			break; // last avaiable table
		DispatchStructType((void*)p);
		LPBYTE nt = p + pHeader->Length; // point to struct end
		while (0 != (*nt | *(nt + 1))) nt++; // skip string area
		nt += 2;
		if (nt >= lastAddress)
			break;
		p = nt;
	}
}

SMBIOS::SMBIOS() :
	m_BIOSSysVersion(0UL),
	m_BIOSECVersion(0UL),
	m_wszBIOSVendor(NULL),
	m_wszBIOSVersion(NULL),
	m_wszBIOSReleaseDate(NULL),
	m_wszSysManufactor(NULL),
	m_wszSysProductName(NULL),
	m_wszSysVersion(NULL),
	m_wszSysSerialNumber(NULL),
	m_SysUUID(GUID_NULL),
	m_wszSysSKU(NULL),
	m_wszSysFamily(NULL),
	m_wszBoardManufactor(NULL),
	m_wszBoardProductName(NULL),
	m_wszBoardVersion(NULL),
	m_wszBoardSerialNumber(NULL),
	m_wszBoardAssetTag(NULL),
	m_wszBoardLocation(NULL)
{
}

SMBIOS::~SMBIOS()
{
	/*if (m_wszBIOSVendor)
		delete m_wszBIOSVendor;
	if (m_wszBIOSVersion)
		delete m_wszBIOSVersion;
	if (m_wszBIOSReleaseDate)
		delete m_wszBIOSReleaseDate;
	if (m_wszSysManufactor)
		delete m_wszSysManufactor;
	if (m_wszSysProductName)
		delete m_wszSysProductName;
	if (m_wszSysVersion)
		delete m_wszSysVersion;
	if (m_wszSysSerialNumber)
		delete m_wszSysSerialNumber;
	if (m_wszSysSKU)
		delete m_wszSysSKU;
	if (m_wszSysFamily)
		delete m_wszSysFamily;
	if (m_wszBoardManufactor)
		delete m_wszBoardManufactor;
	if (m_wszBoardProductName)
		delete m_wszBoardProductName;
	if (m_wszBoardVersion)
		delete m_wszBoardVersion;
	if (m_wszBoardSerialNumber)
		delete m_wszBoardSerialNumber;
	if (m_wszBoardAssetTag)
		delete m_wszBoardAssetTag;
	if (m_wszBoardLocation)
		delete m_wszBoardLocation;*/
}

const SMBIOS & SMBIOS::getInstance(void)
{
	static SMBIOS* pInstance = NULL;
	if (pInstance == NULL)
	{
		// need entry a mutex for thread safe
		if (pInstance == NULL)
		{
			pInstance = new SMBIOS();
			pInstance->initialization();
		}
	}
	return *pInstance;
}

void SMBIOS::initialization(void)
{
	GET_SYSTEM_FIRMWARE_TABLE pGetSystemFirmwareTable = (GET_SYSTEM_FIRMWARE_TABLE)GetProcAddress("kernel32.dll", "GetSystemFirmwareTable");

	LPBYTE pBuff = NULL;

	PBYTE tableStart = nullptr;
	UINT nTableLength = 0;

	if (pGetSystemFirmwareTable)
	{
		DWORD needBufferSize = 0;

		DWORD Signature = 'R';
		Signature = (Signature << 8) + 'S';
		Signature = (Signature << 8) + 'M';
		Signature = (Signature << 8) + 'B';

		needBufferSize = pGetSystemFirmwareTable(Signature, 0, NULL, 0);
		pBuff = new BYTE[needBufferSize];

		pGetSystemFirmwareTable(Signature, 0, pBuff, needBufferSize);

		const PRawSMBIOSData pDMIData = (PRawSMBIOSData)pBuff;
		MajorVersion = pDMIData->MajorVersion;
		MinorVersion = pDMIData->MinorVersion;
		DMIRevision = pDMIData->DmiRevision;

		tableStart = (PBYTE) & (pDMIData->SMBIOSTableData);
		nTableLength = pDMIData->Length;
	}

	if (tableStart)
		ParseSMBIOSStruct(tableStart, nTableLength);

	if (pBuff)
		delete[] pBuff;
}