package server

import (
	"fmt"
	"io"
	"net/http"
	"regexp"
	// "encoding/json"
	// "io/ioutil" ioutil.ReadAll(request.Body)
)

type Server struct {
	mux *http.ServeMux
}

func handleRoot(writer http.ResponseWriter, req *http.Request) {
	fmt.Printf("Request from: %s\n", req.RemoteAddr)
	io.WriteString(writer, fmt.Sprintf("GoLang: Hello from {%v}", req.Context()))
}

func newMux() *http.ServeMux {
	fmt.Printf("Configuring server...\n")
	mux := http.NewServeMux()

	regexpHandler := NewRegexpHandler()
	regexpHandler.HandleFunc(regexp.MustCompile(".*"), handleRoot)

	mux.Handle("/", regexpHandler)
	fmt.Printf("Server has been configured!\n")
	return mux
}

func (s *Server) Run() error {
	err := http.ListenAndServe(":80", s.mux)
	return err
}

func New() *Server {
	mux := newMux()

	return &Server{mux}
}
