package abstractions

import (
	"io"
)

type FileNameGenerator interface {
	GenerateFileName(r io.Reader) string
}
