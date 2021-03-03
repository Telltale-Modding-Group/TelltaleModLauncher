#pragma once

#include <stdio.h>
#include "bytestream.h"

class filestream : public bytestream {
protected:
	FILE* in;
public:
	uint8* read(uint32 size) override;
	~filestream();
	filestream(FILE* f);
	filestream(const char* filepath);
	bool valid() override;
};
