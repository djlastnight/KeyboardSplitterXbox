#pragma once

#include <string>

#include <windows.h>
#include "stdafx.h"
#include "xinput.h"
#include <string>
#include <memory>
#include <Shlwapi.h>
#include <Shlobj.h>
#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "shell32.lib")

class XInputModuleManager
{
public:
	// XInput 1.3 and older functions
	DWORD(WINAPI* XInputGetState)(DWORD dwUserIndex, XINPUT_STATE* pState);
	DWORD(WINAPI* XInputSetState)(DWORD dwUserIndex, XINPUT_VIBRATION* pVibration);
	DWORD(WINAPI* XInputGetCapabilities)(DWORD dwUserIndex, DWORD dwFlags, XINPUT_CAPABILITIES* pCapabilities);
	VOID(WINAPI* XInputEnable)(BOOL enable);
	DWORD(WINAPI* XInputGetDSoundAudioDeviceGuids)(DWORD dwUserIndex, GUID* pDSoundRenderGuid, GUID* pDSoundCaptureGuid);
	DWORD(WINAPI* XInputGetBatteryInformation)(DWORD  dwUserIndex, BYTE devType, XINPUT_BATTERY_INFORMATION* pBatteryInformation);
	DWORD(WINAPI* XInputGetKeystroke)(DWORD dwUserIndex, DWORD dwReserved, PXINPUT_KEYSTROKE pKeystroke);

	// XInput 1.3 undocumented functions
	DWORD(WINAPI* XInputGetStateEx)(DWORD dwUserIndex, XINPUT_STATE *pState); // 100
	DWORD(WINAPI* XInputWaitForGuideButton)(DWORD dwUserIndex, DWORD dwFlag, LPVOID pVoid); // 101
	DWORD(WINAPI* XInputCancelGuideButtonWait)(DWORD dwUserIndex); // 102
	DWORD(WINAPI* XInputPowerOffController)(DWORD dwUserIndex); // 103

	// XInput 1.4 functions
	DWORD(WINAPI* XInputGetAudioDeviceIds)(DWORD dwUserIndex, LPWSTR pRenderDeviceId, UINT* pRenderCount, LPWSTR pCaptureDeviceId, UINT* pCaptureCount);

	// XInput 1.4 undocumented functionss
	DWORD(WINAPI* XInputGetBaseBusInformation)(DWORD dwUserIndex, struct XINPUT_BUSINFO* pBusinfo); // 104
	DWORD(WINAPI* XInputGetCapabilitiesEx)(DWORD unk1, DWORD dwUserIndex, DWORD dwFlags, struct XINPUT_CAPABILITIESEX* pCapabilitiesEx); // 108

	XInputModuleManager()
	{
		std::string current_module;
		ModuleFileName(&current_module, CurrentModule());

		std::string loaded_module_path;
		m_module = LoadLibrarySystem(current_module, &loaded_module_path);

		if (!m_module)
		{
			HRESULT hr = GetLastError();
			std::unique_ptr<char[]> error_msg(new char[MAX_PATH]);
			sprintf_s(error_msg.get(), MAX_PATH, "Cannot load \"%s\" error: 0x%x", loaded_module_path.c_str(), hr);
			MessageBoxA(NULL, error_msg.get(), "Error", MB_ICONERROR);
			exit(hr);
		}

		// XInput 1.3 and older functions
		StoreProcAddress("XInputGetState", &XInputGetState);
		StoreProcAddress("XInputSetState", &XInputSetState);
		StoreProcAddress("XInputGetCapabilities", &XInputGetCapabilities);
		StoreProcAddress("XInputEnable", &XInputEnable);
		StoreProcAddress("XInputGetDSoundAudioDeviceGuids", &XInputGetDSoundAudioDeviceGuids);
		StoreProcAddress("XInputGetBatteryInformation", &XInputGetBatteryInformation);
		StoreProcAddress("XInputGetKeystroke", &XInputGetKeystroke);

		// XInput 1.3 undocumented functions
		StoreProcAddress((const char*)100, &XInputGetStateEx);
		StoreProcAddress((const char*)101, &XInputWaitForGuideButton);
		StoreProcAddress((const char*)102, &XInputCancelGuideButtonWait);
		StoreProcAddress((const char*)103, &XInputPowerOffController);

		// XInput 1.4 functions
		StoreProcAddress("XInputGetAudioDeviceIds", &XInputGetAudioDeviceIds);

		// XInput 1.4 undocumented functionss
		StoreProcAddress((const char*)104, &XInputGetBaseBusInformation);
		StoreProcAddress((const char*)108, &XInputGetCapabilitiesEx);
	}

	~XInputModuleManager()
	{
		if (m_module)
		{
			std::string xinput_path;
			ModulePath(&xinput_path, m_module);
			FreeLibrary(m_module);
		}
	}

	static XInputModuleManager& Get()
	{
		static XInputModuleManager instance;
		return instance;
	}

	bool ModuleFileName(std::string* out, HMODULE hModule)
	{
		std::unique_ptr<char[]> buffer(new char[MAX_PATH]);
		GetModuleFileNameA(hModule, buffer.get(), MAX_PATH);
		*out = PathFindFileNameA(buffer.get());
		return !out->empty();
	}

	bool ModulePath(std::string* out, HMODULE hModule)
	{
		std::unique_ptr<char[]> buffer(new char[MAX_PATH]);
		GetModuleFileNameA(hModule, buffer.get(), MAX_PATH);
		*out = buffer.get();
		return !out->empty();
	}

	inline HMODULE& CurrentModule()
	{
		static HMODULE hModule = 0;
		if (!hModule)
			GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPCTSTR)&hModule, &hModule);
		return hModule;
	}

	HMODULE LoadLibrarySystem(const std::string& library_name, std::string* out_path)
	{
		std::unique_ptr<char[]> system_directory(new char[MAX_PATH]);
		GetSystemDirectoryA(system_directory.get(), MAX_PATH);

		std::string lib_path(system_directory.get());
		StringPathAppend(&lib_path, library_name);

		if (out_path)
			*out_path = lib_path;

		return LoadLibraryA(lib_path.c_str());
	}

	bool StringPathAppend(std::string* path, const std::string& more)
	{
		std::unique_ptr<char[]> buffer(new char[MAX_PATH]);
		*path = PathCombineA(buffer.get(), path->c_str(), more.c_str());
		return !path->empty();
	}

private:
	template<typename T>
	inline void StoreProcAddress(const char* funcname, T* ppfunc)
	{
		*ppfunc = reinterpret_cast<T>(::GetProcAddress(m_module, funcname));
	}

	HMODULE m_module;
};
