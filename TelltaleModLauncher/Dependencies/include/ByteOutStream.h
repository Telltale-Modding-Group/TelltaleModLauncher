#pragma once

#include "LibTelltale.h"
#include <iostream>

class byteoutstream {
public:
	~byteoutstream();
	byteoutstream(uint8* initial_buf, uint64 size);
	byteoutstream(uint32 size);
	byteoutstream();
	uint8* get_buffer();
	uint64 get_stream_size();
	endian get_endian();
	uint64 get_mark();
	void set_endian(endian e);
	void keep_buffer(bool b);//version >= 1.3.0
	void mark_pos(uint64 pos);
	void seek_beg(uint64 pos);
	void write_zeros(uint32 count);
	void seek_cur(uint64 pos);
	void seek_end(uint64 pos);
	void rewind();
	virtual void write(uint8* buf, uint32 size);
	void write_int(uint8 width,uint64 val);
	bool valid();
	uint64 get_position();
protected:
	uint8* buf;
	uint64 mark;
	uint64 size;
	uint64 position;
	endian order;
	bool v;
	bool b;//version >= 1.3.0
	virtual void grow(uint64 dest_size);
};
