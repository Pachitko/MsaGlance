package server

import (
	"bytes"
	"fmt"
	"io"
	"net/http"
	_ "net/http/pprof"
	"tempodisk/internal/dtos/fileDto"
)

func (h *httpHandler) configureMiddlewares() {
	h.router.Use(loggingMiddleware)
}

func (h *httpHandler) configureEndpoints() {
	h.router.HandleFunc("/", h.handleIndex).Methods(http.MethodGet)
	h.router.HandleFunc("/files", h.handleFileUpload).Methods(http.MethodPost)
	h.router.Handle("/debug/pprof/{_:.*}", http.DefaultServeMux).Methods(http.MethodGet)
	h.router.HandleFunc(`/{_:.*}`, h.handleFallback)
}

func (h *httpHandler) handleFileUpload(w http.ResponseWriter, r *http.Request) {
	r.Body = http.MaxBytesReader(w, r.Body, 4<<20+1)
	err := r.ParseMultipartForm(4 << 20) // max 4 mb
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
	}

	var data bytes.Buffer

	file, _, err := r.FormFile("file")
	if err != nil {
		panic(err)
	}
	defer file.Close()

	io.Copy(&data, file)

	tee := io.TeeReader(file, &data)
	fileName := h.fileNameGenerator.GenerateFileName(tee)

	h.filesChan <- fileDto.FileDto{
		FileName: fileName,
		Data:     data,
	}

	fmt.Fprintf(w, "%s", fileName[:10])
}

func (h *httpHandler) handleFallback(w http.ResponseWriter, req *http.Request) {
	http.Error(w, "Fallback", http.StatusNotFound)
}

func (h *httpHandler) handleIndex(w http.ResponseWriter, req *http.Request) {
	io.WriteString(w, fmt.Sprintf("GoLang: Hello from {%v}", req.Context()))
}
