#pragma once

#include "byteoutstream.h"
#include <iostream>

class fileoutstream : public byteoutstream {
public:
	~fileoutstream();
	fileoutstream(FILE* f);
	fileoutstream(const char* filepath);
	FILE* get_out_file();
	void write(uint8* buf, uint32 size) override;
protected:
	void grow(uint64 dest_size) override;
	FILE* file;
};
