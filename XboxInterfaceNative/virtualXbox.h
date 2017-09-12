#pragma once
#include <Xinput.h>

#ifdef VXBOX_EXPORTS
#define VXBOX_API __declspec(dllexport)
#else
#define VXBOX_API __declspec(dllimport)
#endif

//////////// Definitions /////////////////////////

typedef struct _BUSENUM_UNPLUG_HARDWARE {

	__in ULONG Size;

	__in ULONG SerialNo;

	__in ULONG Flags;

	ULONG Reserved[1];

} BUSENUM_UNPLUG_HARDWARE, *PBUSENUM_UNPLUG_HARDWARE;

//{F679F562-3164-42CE-A4DB-E7DDBE723909}  
DEFINE_GUID(GUID_DEVINTERFACE_SCPVBUS, 0xf679f562, 0x3164, 0x42ce, 0xa4, 0xdb, 0xe7, 0xdd, 0xbe, 0x72, 0x39, 0x9);

#define VBOX_BUS
#define FEEDBACK_BUFFER_LENGTH 9
#define MAX_NUMBER_XBOX_CTRLS 4

#define XINPUT_GAMEPAD_GUIDE 0x0400
#define LeftTrigger 0x10000
#define RightTrigger 0x20000
#define XboxAxisX 0x0001
#define XboxAxisY 0x0002
#define XboxAxisRx 0x0004
#define XboxAxisRy 0x0008

#define DPAD_UP XINPUT_GAMEPAD_DPAD_UP
#define DPAD_DOWN XINPUT_GAMEPAD_DPAD_DOWN
#define DPAD_LEFT XINPUT_GAMEPAD_DPAD_LEFT
#define DPAD_RIGHT XINPUT_GAMEPAD_DPAD_RIGHT
#define DPAD_OFF 0

#define AXIS_MAX 32767
#define AXIS_MIN -32768

#define FILE_DEVICE_BUSENUM		FILE_DEVICE_BUS_EXTENDER
#define BUSENUM_IOCTL(_index_)	CTL_CODE(FILE_DEVICE_BUSENUM, _index_, METHOD_BUFFERED, FILE_READ_DATA)
#define BUSENUM_W_IOCTL(_index_)	CTL_CODE(FILE_DEVICE_BUSENUM, _index_, METHOD_BUFFERED, FILE_WRITE_DATA)
#define BUSENUM_R_IOCTL(_index_)	CTL_CODE(FILE_DEVICE_BUSENUM, _index_, METHOD_BUFFERED, FILE_READ_DATA)
#define BUSENUM_RW_IOCTL(_index_)	CTL_CODE(FILE_DEVICE_BUSENUM, _index_, METHOD_BUFFERED, FILE_WRITE_DATA | FILE_READ_DATA)

#define IOCTL_BUSENUM_BASE 0x801

#ifdef VBOX_BUS
#define IOCTL_BUSENUM_PLUGIN_HARDWARE	BUSENUM_W_IOCTL(IOCTL_BUSENUM_BASE+0x0)
#define IOCTL_BUSENUM_UNPLUG_HARDWARE	BUSENUM_W_IOCTL(IOCTL_BUSENUM_BASE+0x1)
#define IOCTL_BUSENUM_EJECT_HARDWARE	BUSENUM_W_IOCTL(IOCTL_BUSENUM_BASE+0x2)
#define IOCTL_BUSENUM_REPORT_HARDWARE	BUSENUM_RW_IOCTL(IOCTL_BUSENUM_BASE+0x3)
#else
#define IOCTL_BUSENUM_PLUGIN_HARDWARE	BUSENUM_IOCTL(0x0)
#define IOCTL_BUSENUM_UNPLUG_HARDWARE	BUSENUM_IOCTL(0x1)
#define IOCTL_BUSENUM_EJECT_HARDWARE	BUSENUM_IOCTL(0x2)
#define IOCTL_BUSENUM_REPORT_HARDWARE	BUSENUM_IOCTL(0x3)
#endif

#define IOCTL_BUSENUM_ISDEVPLUGGED	BUSENUM_RW_IOCTL(IOCTL_BUSENUM_BASE+0x100)
#define IOCTL_BUSENUM_EMPTY_SLOTS	BUSENUM_RW_IOCTL(IOCTL_BUSENUM_BASE+0x101)
#define IOCTL_BUSENUM_PROC_ID		BUSENUM_RW_IOCTL(IOCTL_BUSENUM_BASE+0x102)

//////////// Globals /////////////////////////
XINPUT_GAMEPAD g_Gamepad[MAX_NUMBER_XBOX_CTRLS];
BOOL g_vDevice[MAX_NUMBER_XBOX_CTRLS] = { FALSE };
HANDLE g_hBus = INVALID_HANDLE_VALUE;

#if 0
//////////// Interface Functions /////////////////////////
extern "C"
{
	/// Status
	VXBOX_API BOOL __cdecl VBusExists(void);
	VXBOX_API BOOL __cdecl GetEmptyBusSlotsCount(UCHAR * nSlots);
	VXBOX_API BOOL __cdecl ControllerExists(UINT UserIndex);
	VXBOX_API BOOL __cdecl IsControllerOwned(UINT UserIndex);

	// Virtual device Plug-In/Unplug
	VXBOX_API BOOL __cdecl PlugIn(UINT UserIndex);
	VXBOX_API BOOL __cdecl Unplug(UINT UserIndex, BOOL Force);

	// Data Transfer (Feedback from the device)
	VXBOX_API BOOL __cdecl GetLedNumber(UINT UserIndex, PBYTE pLed);
	VXBOX_API BOOL __cdecl GetVibration(UINT UserIndex, PXINPUT_VIBRATION pVib);
	VXBOX_API BOOL __cdecl GetState(UINT userIndex, XINPUT_GAMEPAD* gamepad);

	// Data Transfer (Data to the device)
	VXBOX_API BOOL __cdecl SetButton(UINT UserIndex, UINT Button, BOOL Press);
	VXBOX_API BOOL __cdecl SetTrigger(UINT UserIndex, UINT Trigger, BYTE Value);
	VXBOX_API BOOL __cdecl SetDpad(UINT UserIndex, INT Value);
	VXBOX_API BOOL __cdecl SetAxis(UINT UserIndex, UINT Axis, SHORT Value);
}  // extern "C"
#endif // 0

//////////// Helper Functions /////////////////////////
BOOL SetAxisX(UINT UserIndex, SHORT Value);
BOOL SetAxisY(UINT UserIndex, SHORT Value);
BOOL SetAxisRx(UINT UserIndex, SHORT Value);
BOOL SetAxisRy(UINT UserIndex, SHORT Value);
BOOL SetTriggerL(UINT UserIndex, BYTE Value);
BOOL SetTriggerR(UINT UserIndex, BYTE Value);

int GetVXbusPath(LPCTSTR path, UINT size);
HANDLE GetVXbusHandle(void);
BOOL GetCreateProcID(DWORD UserIndex, PULONG ProcID);
BOOL XOutputSetState(DWORD UserIndex, XINPUT_GAMEPAD* pGamepad);
BOOL XOutputSetGetState(DWORD UserIndex, XINPUT_GAMEPAD* pGamepad, PBYTE bVibrate, PBYTE bLargeMotor, PBYTE bSmallMotor, PBYTE bLed);
WORD ConvertButton(LONG vBtns, WORD xBtns, UINT vBtn, UINT xBtn);
BOOL UnPlug_Opt(UINT UserIndex, BOOL Force);