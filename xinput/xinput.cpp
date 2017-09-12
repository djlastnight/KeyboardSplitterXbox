#include "stdafx.h"
#include "xinput.h"
#include "XInputModuleManager.h"
#include "IniReader.h"

#include <iostream>
#include <sstream>
using namespace std;
#define XINPUT_DEVSUBTYPE_NOTSET 0x17
#define XINPUT_SETTINGS_FILENAME "xinput.ini"

extern "C" DWORD WINAPI XInputGetState(_In_ DWORD dwUserIndex, _Out_ XINPUT_STATE* pState)
{
	return XInputModuleManager::Get().XInputGetState(dwUserIndex, pState);
}

extern "C" DWORD WINAPI XInputSetState(_In_ DWORD dwUserIndex, _In_ XINPUT_VIBRATION* pVibration)
{
	return XInputModuleManager::Get().XInputSetState(dwUserIndex, pVibration);
}

extern "C" DWORD WINAPI XInputGetCapabilities(_In_ DWORD dwUserIndex, _In_ DWORD dwFlags, _Out_ XINPUT_CAPABILITIES* pCapabilities)
{
	if (!pCapabilities || dwFlags != 0 && dwFlags != XINPUT_FLAG_GAMEPAD)
	{
		pCapabilities = NULL;
		return ERROR_BAD_ARGUMENTS;
	}

	DWORD result = XInputModuleManager::Get().XInputGetCapabilities(dwUserIndex, dwFlags, pCapabilities);
	if (result == ERROR_SUCCESS)
	{
		INIReader reader(XINPUT_SETTINGS_FILENAME);
		if (reader.ParseError() < 0)
		{
			return result;
		}

		ostringstream stream;
		stream << dwUserIndex + 1;
		int subType = reader.GetInteger("SubTypes", stream.str(), XINPUT_DEVSUBTYPE_NOTSET);

		switch (subType)
		{
		case XINPUT_DEVSUBTYPE_UNKNOWN:
		case XINPUT_DEVSUBTYPE_GAMEPAD:
		case XINPUT_DEVSUBTYPE_WHEEL:
		case XINPUT_DEVSUBTYPE_ARCADE_STICK:
		case XINPUT_DEVSUBTYPE_FLIGHT_STICK:
		case XINPUT_DEVSUBTYPE_DANCE_PAD:
		case XINPUT_DEVSUBTYPE_GUITAR:
		case XINPUT_DEVSUBTYPE_GUITAR_ALTERNATE:
		case XINPUT_DEVSUBTYPE_DRUM_KIT:
		case XINPUT_DEVSUBTYPE_GUITAR_BASS:
		case XINPUT_DEVSUBTYPE_ARCADE_PAD:
			pCapabilities->SubType = subType;
			break;
		default:
			break;
		}


	}

	return result;
}

extern "C" VOID WINAPI XInputEnable(_In_ BOOL enable)
{
	XInputModuleManager::Get().XInputEnable(enable);
}

extern "C" DWORD WINAPI XInputGetDSoundAudioDeviceGuids(DWORD dwUserIndex, GUID* pDSoundRenderGuid, GUID* pDSoundCaptureGuid)
{
	return XInputModuleManager::Get().XInputGetDSoundAudioDeviceGuids(dwUserIndex, pDSoundRenderGuid, pDSoundCaptureGuid);
}

extern "C" DWORD WINAPI XInputGetBatteryInformation(_In_ DWORD dwUserIndex, _In_ BYTE devType, _Out_ XINPUT_BATTERY_INFORMATION* pBatteryInformation)
{
	return XInputModuleManager::Get().XInputGetBatteryInformation(dwUserIndex, devType, pBatteryInformation);
}

extern "C" DWORD WINAPI XInputGetKeystroke(_In_ DWORD dwUserIndex, _Reserved_ DWORD dwReserved, _Out_ XINPUT_KEYSTROKE* pKeystroke)
{
	return XInputModuleManager::Get().XInputGetKeystroke(dwUserIndex, dwReserved, pKeystroke);
}

//undocumented
extern "C" DWORD WINAPI XInputGetStateEx(DWORD dwUserIndex, XINPUT_STATE *pState)
{
	return XInputModuleManager::Get().XInputGetStateEx(dwUserIndex, pState);
}

extern "C" DWORD WINAPI XInputWaitForGuideButton(DWORD dwUserIndex, DWORD dwFlag, LPVOID pVoid)
{
	return XInputModuleManager::Get().XInputWaitForGuideButton(dwUserIndex, dwFlag, pVoid);
}

extern "C" DWORD WINAPI XInputCancelGuideButtonWait(DWORD dwUserIndex)
{
	return XInputModuleManager::Get().XInputCancelGuideButtonWait(dwUserIndex);
}

extern "C" DWORD WINAPI XInputPowerOffController(DWORD dwUserIndex)
{
	return XInputModuleManager::Get().XInputPowerOffController(dwUserIndex);
}

extern "C" DWORD WINAPI XInputGetAudioDeviceIds(_In_ DWORD dwUserIndex, _Out_writes_opt_(*pRenderCount) LPWSTR pRenderDeviceId, _Inout_opt_ UINT* pRenderCount, _Out_writes_opt_(*pCaptureCount) LPWSTR pCaptureDeviceId, _Inout_opt_ UINT* pCaptureCount)
{
	return XInputModuleManager::Get().XInputGetAudioDeviceIds(dwUserIndex, pRenderDeviceId, pRenderCount, pCaptureDeviceId, pCaptureCount);
}

extern "C" DWORD WINAPI XInputGetBaseBusInformation(DWORD dwUserIndex, struct XINPUT_BUSINFO* pBusinfo)
{
	return XInputModuleManager::Get().XInputGetBaseBusInformation(dwUserIndex, pBusinfo);
}

// XInput 1.4 uses this in XInputGetCapabilities and calls memcpy(pCapabilities, &CapabilitiesEx, 20u);
// so XINPUT_CAPABILITIES is first 20 bytes of XINPUT_CAPABILITIESEX
extern "C" DWORD WINAPI XInputGetCapabilitiesEx(DWORD unk1 /*seems that only 1 is valid*/, DWORD dwUserIndex, DWORD dwFlags, struct XINPUT_CAPABILITIESEX* pCapabilitiesEx)
{
	return XInputModuleManager::Get().XInputGetCapabilitiesEx(unk1, dwUserIndex, dwFlags, pCapabilitiesEx);
}