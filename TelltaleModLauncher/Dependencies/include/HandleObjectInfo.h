#pragma once
#include <vector>
#include "ByteStream.h"
#include "ByteOutStream.h"
#include "TTArchive.h"
#include "TTContext.h"

template<typename T> class Baseclass_HandleBase {
public:
	TTArchiveOrTTArchive2 archive;
	char* mName;
	uint64 mNameCRC;

	Baseclass_HandleBase(T value) : value(value) {};

	Baseclass_HandleBase(TTArchiveOrTTArchive2 ar, char* name, uint64 crc) : archive(ar);

	Baseclass_HandleBase(ttarchive::TTArchive* archive, char* name, uint64 crc);

	Baseclass_HandleBase(ttarchive2::TTArchive2* archive, uint64 crc, char* name);

	T GetValue();

protected:
	T value;
};

class TTContext;

template<typename T> class Baseclass_ContainerInterface {
public:
	int mCapacity;
	int mSize;
protected:
	std::vector<T> backend;
public:

	Baseclass_ContainerInterface() : backend(std::vector<T>());


	~Baseclass_ContainerInterface();
};

template<typename T> class SArray : public Baseclass_ContainerInterface<T> {
public:

	SArray() : Baseclass_ContainerInterface<T>() {};

	int Size();

	const T& operator[](std::size_t idx);

};

template<typename T> class DCArray : public SArray<T> {
public:

	DCArray<T>() : SArray<T>() {};

	void Add(const T& v);


	void Remove(T& v);

	void AddAll(const SArray<T>& other);

	void Clear();
};