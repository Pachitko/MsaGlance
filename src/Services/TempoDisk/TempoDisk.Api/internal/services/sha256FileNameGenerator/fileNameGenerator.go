package sha256FileNameGenerator

import (
	"crypto/sha256"
	"encoding/hex"
	"io"
	"log"
)

type Sha256FileNameGenerator struct {
}

func New() *Sha256FileNameGenerator {
	return &Sha256FileNameGenerator{}
}

func (g *Sha256FileNameGenerator) GenerateFileName(r io.Reader) string {
	sha256 := sha256.New()
	if _, err := io.Copy(sha256, r); err != nil {
		log.Fatal(err)
	}
	return hex.EncodeToString(sha256.Sum(nil))
}
