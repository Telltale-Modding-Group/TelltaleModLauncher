#pragma once 

#include "LibTelltale.h"
#include <iostream>

typedef uint8* (*convert_func)(uint8*, uint32, uint32, uint32*);

class bytestream {
protected:
	endian order;
	uint64 size;
	bool b;//version >= 1.3.0
	uint64 pos;
	uint64 mark;
	uint8* buf = nullptr;
public:
	bytestream();
	bytestream(uint8* buf, uint64 size);
	bytestream(const char* str);
	~bytestream();
	virtual uint8 read();
	virtual uint8* read(uint32 size);
	unsigned char* read_string(uint32 len);
	unsigned char* read_string();
	uint64 read_int(uint8 width);
	void read_to(uint8* buf, uint32 size);
	void read_to_filter(uint8* buf, uint32 size, uint32 chunk_size, convert_func func, uint32* outsize);
	void mark_pos(uint64 pos);
	uint64 get_position();
	uint64 get_stream_size();
	uint64 get_mark();
	uint8* get_buffer();
	void rewind();
	endian get_endian();
	void set_endian(endian e);
	void keep_buffer(bool b);//version >= 1.3.0
	virtual bool seek_beg(uint64 pos);
	bool seek_cur(uint64 pos);
	bool seek_end(uint64 pos);
	virtual bool valid();
};
