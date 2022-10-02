package server

import (
	"net/http"
	"tempodisk/internal/abstractions"
	"tempodisk/internal/dtos/fileDto"

	"github.com/gorilla/mux"
)

type httpHandler struct {
	router            *mux.Router
	filesChan         chan<- fileDto.FileDto
	fileNameGenerator abstractions.FileNameGenerator
}

func (h *httpHandler) ServeHTTP(w http.ResponseWriter, req *http.Request) {
	h.router.ServeHTTP(w, req)
}

func NewHttpHandler(filesChan chan<- fileDto.FileDto, fileNameGenerator abstractions.FileNameGenerator) *httpHandler {
	router := mux.NewRouter()

	httpHandler := &httpHandler{router, filesChan, fileNameGenerator}
	httpHandler.configureMiddlewares()
	httpHandler.configureEndpoints()

	return httpHandler
}
