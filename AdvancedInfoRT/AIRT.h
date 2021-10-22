#pragma once
#include <collection.h>
#include <windows.h>

namespace AdvancedInfoRT
{
	typedef struct _MEMORYSTATUSEX {
		DWORD     dwLength;
		DWORD     dwMemoryLoad;
		DWORDLONG ullTotalPhys;
		DWORDLONG ullAvailPhys;
		DWORDLONG ullTotalPageFile;
		DWORDLONG ullAvailPageFile;
		DWORDLONG ullTotalVirtual;
		DWORDLONG ullAvailVirtual;
		DWORDLONG ullAvailExtendedVirtual;
	} MEMORYSTATUSEX;
	typedef MEMORYSTATUSEX* PMEMORYSTATUSEX;

	typedef BOOL(STDAPICALLTYPE GLOBALMEMORYSTATUSEX)
		(
			IN MEMORYSTATUSEX *lpBuffer
		);
	typedef GLOBALMEMORYSTATUSEX FAR* LPGLOBALMEMORYSTATUSEX;

    public ref class AIRT sealed
    {
	private:
		property LPGLOBALMEMORYSTATUSEX GlobalMemoryStatusEx;

    public:
        AIRT();
		void	AIRT::InitNTDLLEntryPoints();
		Platform::Boolean AIRT::GetSystemRAM(int64 *SystemRAM);
		Platform::String^ AIRT::GetSystemManufacturer();
		Platform::String^ AIRT::GetSystemModel();
		Platform::String^ AIRT::GetSystemSku();
		Platform::String^ AIRT::GetSystemFirmwareVersion();
		int AIRT::GetFirmwareVersion();
    };
}
