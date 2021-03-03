#pragma once

/*
	Helper functions for programs in languages such as C#, Java and Python to access the classes and their functions.
	h Prefix = helper function
	Ones without a h prefix are for array handling for template specific type :D

*/

#include "LibTelltale.h"
#include "ByteStream.h"
#include "ByteOutStream.h"
#include "FileStream.h"
#include "InputMapper.h"
#include "FileOutStream.h"
#include "TTArchive.h"
#include "MetaStream.h"
#include "HandleObjectInfo.h"
#include "TTContext.h"

#define E _LIBTT_EXPORT

#define CLASS_DEL(_C,_N) E void h##_N## _Delete(_C v) { delete v; };

#define DCARRAY_DECL_EDIT(_T, _PREF) \
	DCARRAY_DECL(_T,_PREF) \
	E void _PREF ## _DCArray_Add(DCArray<_T>* arr, _T value) { arr->Add(value); };\
	E void _PREF ## _DCArray_Remove(DCArray<_T>* arr, _T value) { arr->Remove(value); };

#define DCARRAY_DECL(_T, _PREF) \
E void _PREF ## _DCArray_Clear(DCArray<_T>* arr){ arr->Clear(); } \
DCARRAY_DECL_NOCLEAR(_T, _PREF)

#define DCARRAY_DECL_NOCLEAR(_T, _PREF) \
E int _PREF ## _DCArray_Size(DCArray<_T>* arr){ return arr->Size(); }\
E _T _PREF ## _DCArray_At(DCArray<_T>* arr, int index) { return arr->operator[](index); }; 

DCARRAY_DECL_NOCLEAR(Vers*, VersBlocks)

//IMAP

DCARRAY_DECL_EDIT(InputMapper::EventMapping*, InputMapper)

E InputMapper* hInputMapper_Create() { return new InputMapper(); }

E DCArray<InputMapper::EventMapping*>* hInputMapper_Mappings(InputMapper* imap) { return imap->mappings; };

CLASS_DEL(InputMapper*, InputMapper)

E InputMapper::EventMapping* hInputMapping_CreateMapping() { return new InputMapper::EventMapping(); };

//META

DCARRAY_DECL_EDIT(MetaClassDescription*, MetaStreamClasses)

CLASS_DEL(MetaStream*, MetaStream)

E void hMemory_Free(void* ptr) { free(ptr); };

E void* hMemory_Alloc(uint32 size) { return (uint8*)malloc(size); }

E void hMemory_FreeArray(char* str) { delete[] str; }

E char* hMemory_CreateArray(const char f[]) {  return _strdup(f); };

E void hMetaStream_SetVersion(MetaStream* stream, int version);

E void hMetaStream_SetFlags(MetaStream* stream, int flags);

E MetaStream* hMetaStream_Create();

E void hMetaStream_Flush(MetaStream* stream, byteoutstream* stream1);

E bool hMetaStream_Open(MetaStream * s, bytestream* strm);

E void hMetaStream_Close(MetaStream* stream);

E uint32 hMetaStream_GetVersion(MetaStream* stream);

E uint32 hMetaStream_GetFlags(MetaStream* stream);

E uint32 hMetaStream_GetPayloadSize(MetaStream* stream);

E uint32 hMetaStream_GetTextureSize(MetaStream* stream);

E uint32 hMetaStream_GetClassVersion(MetaStream* stream, const char typeName[], uint32 def);

E DCArray<MetaClassDescription*>* hMetaStream_GetClasses(MetaStream* stream);

//TTCTX

CLASS_DEL(TTContext*, TTContext)

CLASS_DEL(TTArchiveOrTTArchive2*, TTArchiveOrTTArchive2)

E TTArchiveOrTTArchive2* hTTArchiveOrTTArchive2_Create() { return new TTArchiveOrTTArchive2(); };

E char* hTTContext_FindArchiveEntry(TTContext* ctx, uint64 crc);

E TTContext* hTTContext_Create(const char file[], TTArchiveOrTTArchive2* arc);

E void hTTContext_NextArchive(const char file[],TTContext* ctx, TTArchiveOrTTArchive2* archive, bool del);

E bool hTTContext_NextStream(TTContext* p, bytestream* stream, bool del);

E bool hTTContext_NextWrite(TTContext* p, byteoutstream* stream, const char file[], bool del);

E void hTTContext_OverrideMeta(TTContext* ctx, MetaStream* meta, bool del);

E void hTTContext_FinishWrite(TTContext* ctx, bool del, bool u);

E char* hTTContext_CurrentGameID(TTContext* ctx) { return ctx->GetCurrentGameID(); };

E void hTTContext_NextGame(TTContext* ctx, const char gameID[]) { ctx->NextGame(gameID); };

E char* hTTContext_CurrentFile(TTContext* ctx);

E uint32 hTTContext_FileStart(TTContext* p);

E byteoutstream* hTTContext_CurrentOutStream(TTContext* p);

E bytestream* hTTContext_CurrentStream(TTContext* p);

E MetaStream* hTTContext_CurrentMeta(TTContext* p);

E bytestream* hTTContext_OpenStream(TTContext* p, const char file[]);

// TTARCHIVES

CLASS_DEL(ttarchive::TTArchive*, TTArchive)

CLASS_DEL(ttarchive2::TTArchive2*, TTArchive2)

E void hTTArchive_EntryRemove(ttarchive::TTArchive* archive, ttarchive::TTArchiveEntry* entry, bool free_entry);

E void hTTArchive2_EntryRemove(ttarchive2::TTArchive2* archive, ttarchive2::TTArchive2Entry* entry, bool free_entry);

E void hTTArchive_EntrySetName(ttarchive::TTArchiveEntry* entry, const char name[]);

E void hTTArchive_EntryAdd(ttarchive::TTArchive* arc, ttarchive::TTArchiveEntry* entry);

E void hTTArchive2_EntryAdd(ttarchive2::TTArchive2* arc, ttarchive2::TTArchive2Entry* entry);

E ttarchive2::TTArchive2* hTTArchive2_Create();

E ttarchive::TTArchive* hTTArchive_Create();

E uint32 hTTArchive_GetEntryCount(ttarchive::TTArchive* archive);

E ttarchive::TTArchiveEntry* hTTArchive_GetEntryAt(ttarchive::TTArchive* archive, uint32 index);

E void hTTArchive_ClearEntries(ttarchive::TTArchive* archive);

E uint32 hTTArchive2_GetEntryCount(ttarchive2::TTArchive2* archive);

E ttarchive2::TTArchive2Entry* hTTArchive2_GetEntryAt(ttarchive2::TTArchive2* archive, uint32 index);

E void hTTArchive2_ClearEntries(ttarchive2::TTArchive2* archive);

// BYTE OUT STREAMS

uint8* hByteOutStream_GetBuffer(byteoutstream* stream);

E void hByteOutStream_Position(byteoutstream* stream, uint64 off);

E bool hByteOutStream_IsLittleEndian(byteoutstream* stream);

E void hByteOutStream_SetEndian(byteoutstream* stream, bool little);

E void hByteOutStream_WriteInt(byteoutstream* stream, uint32 width, uint64 i);

E uint64 hByteOutStream_GetPosition(byteoutstream* stream);

E uint64 hByteOutStream_GetSize(byteoutstream* stream);

E void hByteOutStream_WriteBytes(byteoutstream* stream, uint8* buf, uint32 size);

E byteoutstream* hByteOutStream_Create(uint32 size);

E fileoutstream* hFileOutStream_Create(const char filepath[]);

E bool hByteOutStream_Valid(byteoutstream* stream);

CLASS_DEL(byteoutstream*, ByteOutStream)

//BYTE STREAMS

uint8* hByteStream_GetBuffer(bytestream* stream);

E void hByteStream_Position(bytestream* stream, uint64 off);

E bool hByteStream_IsLittleEndian(bytestream* stream);

E void hByteStream_SetEndian(bytestream* stream, bool little);

E uint64 hByteStream_ReadInt(bytestream* stream, uint32 width);

E uint64 hByteStream_GetPosition(bytestream* stream);

E uint64 hByteStream_GetSize(bytestream* stream);

E unsigned char* hByteStream_ReadString(bytestream* stream, uint32 len);

E unsigned char* hByteStream_ReadString0(bytestream* stream);

E uint8* hByteStream_ReadBytes(bytestream* stream, uint32 size);

E uint8 hByteStream_ReadByte(bytestream* stream);

E bytestream* hByteStream_Create(uint32 size);

E bytestream* hByteStream_CreateFromBuffer(uint8* buf, uint32 size);

E filestream* hFileStream_Create(const char filepath[]);

E bool hByteStream_Valid(bytestream* stream);

CLASS_DEL(bytestream*, ByteStream)
