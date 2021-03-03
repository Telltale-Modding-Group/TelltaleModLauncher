#pragma once

#include "LibTelltale.h"
#include "HandleObjectInfo.h"
#include "TTContext.h"

#define INPUTMAPPER_EXT imap

//Most keys are just ASCII (0 = 0x30, = 48 etc for numbers), but a mapping is easier
enum InputCode {
	BACKSPACE = 8,
	NUM_0 = 48,
	NUM_1 = 49,
	NUM_2 = 50,
	NUM_3 = 51,
	NUM_4 = 52,
	NUM_5 = 53,
	NUM_6 = 54,
	NUM_7 = 55,
	NUM_8 = 56,
	NUM_9 = 57,
	BUTTON_A = 512,
	BUTTON_B = 513,
	BUTTON_X = 514,
	BUTTON_Y = 515,
	BUTTON_L = 516,
	BUTTON_R = 517,
	BUTTON_BACK = 518,
	BUTTON_START = 519,
	TRIGGER_L = 520,
	TRIGGER_R = 521,
	DPAD_UP = 524,
	DPAD_DOWN = 525,
	DPAD_RIGHT = 526,
	DPAD_LEFT = 527,
	CONFIRM = 768,
	CANCEL = 769,
	MOUSE_MIDDLE = 770,
	MOUSE_LEFT_DOUBLE = 772,
	MOUSE_MOVE = 784,
	ROLLOVER = 800,
	WHEEL_UP = 801,
	WHEEL_DOWN = 802,
	MOUSE_LEFT = 816,
	MOUSE_RIGHT = 817,
	LEFT_STICK = 1024,
	RIGHT_STICK = 1025,
	TRIGGER = 1026,
	TRIGGER_LEFT_MOVE = 1027,
	TRIGGER_RIGHT_MOVE = 1028,
	SWIPE_LEFT = 1296,
	SWIPE_RIGHT = 1298,
	DOUBLE_TAP_SCREEN = 1304,
	SHIFT = 16,
	ENTER = 13,
	ESCAPE = 27,
	LEFT_ARROW = 37,
	RIGHT_ARROW = 39,
	DOWN_ARROW = 40,
	UP_ARROW = 38,
	KEY_A = 65,
	KEY_B = 66,
	KEY_C = 67,
	KEY_D = 68,
	KEY_E = 69,
	KEY_F = 70,
	KEY_G = 71,
	KEY_H = 72,
	KEY_I = 73,
	KEY_J = 74,
	KEY_K = 75,
	KEY_L = 76,
	KEY_M = 77,
	KEY_N = 78,
	KEY_O = 79,
	KEY_P = 80,
	KEY_Q = 81,
	KEY_R = 82,
	KEY_S = 83,
	KEY_T = 84,
	KEY_U = 85,
	KEY_V = 86,
	KEY_W = 87,
	KEY_X = 88,
	KEY_Y = 89,
	KEY_Z = 90,
};

enum Event {
	BEGIN = 0,
	END = 1
};

class InputMapper {
public:

	struct EventMapping {
		InputCode mInputCode;
		Event mEvent;
		char* mScriptFunction;
		int32 mControllerIndexOverride;
	};

	InputMapper() {
		this->mappings = new DCArray<EventMapping*>();
	};
	~InputMapper() {
		for (int i = 0; i < this->mappings->Size(); i++){
			delete[] this->mappings->operator[](i)->mScriptFunction;
			delete this->mappings->operator[](i);
		}
		this->mappings->Clear();
		delete this->mappings;
	};
	DCArray<EventMapping*>* mappings;
};

_LIBTT_EXPORT int InputMapper_Open(TTContext* ctx, InputMapper* imap);
_LIBTT_EXPORT bool InputMapper_Flush(TTContext* ctx, InputMapper* imap);