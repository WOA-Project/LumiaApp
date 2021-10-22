#include "pch.h"
#include "AIRT.h"
#include "smbios.h"

using namespace AdvancedInfoRT;
using namespace Platform;
using namespace SMBIOSRT;

AIRT::AIRT()
{
}

void AIRT::InitNTDLLEntryPoints()
{
	GlobalMemoryStatusEx = (LPGLOBALMEMORYSTATUSEX)GetProcAddress("kernel32.dll", "GlobalMemoryStatusEx");
	if (!GlobalMemoryStatusEx) {
		throw ref new Platform::COMException(HRESULT_FROM_WIN32(GetLastError()));
	}
}

Platform::Boolean AIRT::GetSystemRAM(int64* SystemRAM)
{
	*SystemRAM = 0;
	MEMORYSTATUSEX memStatus;
	memStatus.dwLength = sizeof(memStatus);
	bool result = GlobalMemoryStatusEx(&memStatus);

	if (result)
		*SystemRAM = memStatus.ullTotalPhys;

	return result;
}

Platform::String^ AIRT::GetSystemManufacturer()
{
	SMBIOS smb = SMBIOS::getInstance();
	return ref new String(smb.SysManufactor());
}

Platform::String^ AIRT::GetSystemModel()
{
	SMBIOS smb = SMBIOS::getInstance();
	return ref new String(smb.SysProductName());
}

Platform::String^ AIRT::GetSystemSku()
{
	SMBIOS smb = SMBIOS::getInstance();
	return ref new String(smb.SysSKU());
}

Platform::String^ AIRT::GetSystemFirmwareVersion()
{
	SMBIOS smb = SMBIOS::getInstance();
	return ref new String(smb.BIOSVersion());
}

int AIRT::GetFirmwareVersion()
{
	SMBIOS smb = SMBIOS::getInstance();
	return smb.BIOSSysVersion();
}