package fileDto

import (
	"bytes"
)

type FileDto struct {
	FileName string
	Data     bytes.Buffer
}

func NewFile(filename string, data bytes.Buffer) *FileDto {
	return &FileDto{filename, data}
}
