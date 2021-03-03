#pragma once

#include "LibTelltale.h"
#include "HandleObjectInfo.h"
#include "LibTelltale.h"
#include "TTContext.h"

//.vers files are in old games and specify the variables and they're types to read meta streamed files as

/*SerializedVersionInfo/MetaVersionInfo = .vers */

class TTContext;

template<typename T>
class DCArray;

//Index based
#define VERS_GETBLOCKNAMEI(vers,i) vers->mBlockVarNames ? "_uninitialized" : vers->mBlockVarNames[i]

_LIBTT_EXPORT const char* CRCToClass(uint64 crc);

_LIBTT_EXPORT uint64 ClassToCRC(char* clazz);

/*_LIBTT_EXPORT*/ uint32 VersionToCRC(uint32 unknown);//TODO change type?
/*_LIBTT_EXPORT*/ uint32 CRCToVersion(uint32 crc);//TODO change type?

typedef struct Vers {
	char* mTypeName;
	char* mFullTypeName;
	bool mbBlocked;
	uint32 mBlockLength;
	uint32 mVersion;
	char** mBlockVarNames;
	DCArray<Vers*>* mBlocks;
	TTContext* ctx;
	Vers() {
		mbBlocked = false;
		mBlockLength = 0;
		mTypeName = NULL;
		mFullTypeName = NULL;
		mVersion = 0;
		ctx = NULL;
		mBlocks = NULL;
		mBlockVarNames = NULL;
	};
} Vers;

_LIBTT_EXPORT Vers* Vers_Create(TTContext* ctx);
_LIBTT_EXPORT int Vers_Open(Vers* out);
_LIBTT_EXPORT bool Vers_Flush(Vers* vers, TTContext* ctx);
_LIBTT_EXPORT void Vers_Free(Vers* out);