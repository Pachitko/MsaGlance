package server

import (
	"net/http"

	"github.com/gorilla/mux"
	// "encoding/json"
	// "io/ioutil" ioutil.ReadAll(request.Body)
)

type HttpHandlerConfiguration struct {
}

type httpHandler struct {
	router *mux.Router
	config HttpHandlerConfiguration
}

func (h *httpHandler) ServeHTTP(w http.ResponseWriter, req *http.Request) {
	h.router.ServeHTTP(w, req)
}

func NewHttpHandler(config HttpHandlerConfiguration) *httpHandler {
	router := mux.NewRouter()

	httpHandler := &httpHandler{router, config}
	httpHandler.configureEndpoints()

	return httpHandler
}
