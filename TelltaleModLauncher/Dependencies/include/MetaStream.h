#pragma once

#include "LibTelltale.h"
#include "ByteStream.h"
#include "HandleObjectInfo.h"
#include "ByteOutStream.h"

//Once your TTContext is writing a file, the meta stream CANNOT be edited or the file will come out corrupt. The way to do it is:
/*
* - NextWrite (ttcontext)
* - <Write to stream> eg Vers_Flush
* - FinishCurrentWrite (ttcontext) - This removes the out stream ptr (so you need to call next write to write again) and the meta start offset.
* 
* NOTE TO ME: when doing d3dtx make sure that the texture SIZE is set in the metastream upon FLUSH (context->current meta), and thats its NOT 0. if it is throw an error
* also meta stream, on next write make sure that ppl know to have the meta stream set otherwise the meta header will be emptry and it wont load (useful for debugging only)
* when creating for .bundle files, the flags are 8 and the texture size is the inner meta streamed file. may have to make a workaround
* 
* All versions of meta streams are:
* MBES, MBIN, MCOM, MSV4, MSV5, MSV6. MBES is an encrypted stream handled and converted to MBIN and automatically converted back during ttarchive read and write. mcom and mvs4 havent been released
* and i havent seen them so support for them isnt available D:
*
* 
*/

template<typename T>
class DCArray;

typedef struct {
	char* mTypeName;
	uint32 mVersion;
	uint64 mTypeNameCrc;
	uint32 mVersionCrc;
} MetaClassDescription;

class TTContext;

class MetaStream {
protected:
	DCArray<MetaClassDescription*>* classes;
public:
	void Flush(byteoutstream* stream);
	uint32  mMetaVersion;
	uint32 mFlags;
	uint32 mSize;
	uint32 mTextureSize;
	MetaStream();
	~MetaStream();
	void Close();
	bool Open(bytestream* stream);
	DCArray<MetaClassDescription*>* GetClasses();
	uint32 GetClassVersion(const char typeName[], uint32 def);

};
