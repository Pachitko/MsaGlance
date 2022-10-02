package fileSavers

import (
	"context"
	"log"
	"os"
	"path"
	"sync"
	"tempodisk/internal/abstractions"
	"tempodisk/internal/dtos/fileDto"
)

type fileSaverPool struct {
	n         int64
	filesChan <-chan fileDto.FileDto
	wg        *sync.WaitGroup
	fileDir   string
}

func NewPool(N int64, filesChan chan fileDto.FileDto, fileDir string) abstractions.Pool {
	err := os.MkdirAll(fileDir, os.ModePerm)
	if err != nil {
		log.Printf("File saver pool error '%v'\n", err)
	}

	return &fileSaverPool{N, filesChan, new(sync.WaitGroup), fileDir}
}

func (p *fileSaverPool) Start(ctx context.Context) {
	for i := int64(0); i < p.n; i++ {
		p.wg.Add(1)
		go func() {
			defer p.wg.Done()
			select {
			case file := <-p.filesChan:
				defer file.Data.Reset()

				filePath := path.Join(p.fileDir, file.FileName)
				if _, err := os.Stat(filePath); err == nil {
					log.Printf("File '%s' already exists", filePath)
					return
				}

				err := os.WriteFile(filePath, file.Data.Bytes(), os.FileMode(0600))
				if err != nil {
					log.Printf("File save error: '%v'", err)
				} else {
					log.Printf("File saved with name '%s'", file.FileName)
				}
			case <-ctx.Done():
				return
			}
		}()
	}
}

func (p *fileSaverPool) Wait() {
	p.wg.Wait()
}
