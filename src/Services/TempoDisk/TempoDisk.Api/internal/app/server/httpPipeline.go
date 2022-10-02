package server

import (
	"fmt"
	"io"
	"net/http"
	_ "net/http/pprof"
)

func (h *httpHandler) configureEndpoints() {
	h.router.HandleFunc("/", h.handleIndex).Methods(http.MethodGet)
	h.router.Handle("/debug/pprof/{_:.*}", http.DefaultServeMux).Methods(http.MethodGet)
	h.router.HandleFunc(`/{_:.*}`, h.handleFallback)
}

func (h *httpHandler) handleFallback(w http.ResponseWriter, req *http.Request) {
	w.WriteHeader(http.StatusNotFound)
	io.WriteString(w, "Fallback")
}

func (h *httpHandler) handleIndex(w http.ResponseWriter, req *http.Request) {
	io.WriteString(w, fmt.Sprintf("GoLang: Hello from {%v}", req.Context()))
}
