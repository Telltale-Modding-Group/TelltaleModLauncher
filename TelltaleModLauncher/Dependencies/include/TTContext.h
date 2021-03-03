#pragma once

#include "LibTelltale.h"
#include "TTArchive.h"
#include "ByteStream.h"
#include "MetaStream.h"
#include "Vers.h"
#include "HandleObjectInfo.h"
#include "TTArchive.h"

typedef struct Block {
	uint32 size;
	bytestream* blockstream;
} Block;

struct Vers;
class MetaStream;
template<typename T>
class DCArray;

class TTContext {
	protected:
		TTArchiveOrTTArchive2* archive;
		bytestream* current_stream;
		byteoutstream* current_ostream;
		uint32 start_offset;
		MetaStream* current_meta;
		char* current_file_name; 
		char* current_gameid;
	public:
		TTContext(const char f[],TTArchiveOrTTArchive2* arc);
		void NextArchive(const char ID[], TTArchiveOrTTArchive2* arc, bool del);
		void NextGame(const char ID[]);
		~TTContext();
		bool NextStream(bytestream* next, bool del);
		bool NextWrite(const char file[],byteoutstream* out, bool del);
		void OverrideNewMeta(MetaStream* stream, bool del);
		void FinishCurrentWrite(bool del, bool updateArchive);
		char* GetCurrentFileName();
		Block* ReadBlock();
		void SkipBlock();
		void SkipString();
		char* ReadString();
		char* GetCurrentGameID();
		void DeleteBlock(Block* blk);
		char* FindArchiveEntry(uint64 filename_crc);
		uint32 GetStartOffset();
		MetaStream* GetCurrentMeta();
		bytestream* GetCurrentStream();
		byteoutstream* GetCurrentOutStream();
		bytestream* OpenArchiveStream(const char fileName[]);
		DCArray<Vers*>* versions;

};