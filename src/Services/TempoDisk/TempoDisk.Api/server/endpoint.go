package server

import (
	"net/http"
	"regexp"
)

type Endpoint struct {
	pattern *regexp.Regexp
	handler http.Handler
}

type RegexpHandler struct {
	endpoints []*Endpoint
}

func NewRegexpHandler() *RegexpHandler {
	return &RegexpHandler{}
}

func (h *RegexpHandler) Handler(pattern *regexp.Regexp, handler http.Handler) {
	h.endpoints = append(h.endpoints, &Endpoint{pattern, handler})
}

func (h *RegexpHandler) HandleFunc(pattern *regexp.Regexp, handler func(http.ResponseWriter, *http.Request)) {
	h.endpoints = append(h.endpoints, &Endpoint{pattern, http.HandlerFunc(handler)})
}

func (h *RegexpHandler) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	for _, route := range h.endpoints {
		if route.pattern.MatchString(r.URL.Path) {
			route.handler.ServeHTTP(w, r)
			return
		}
	}

	http.NotFound(w, r)
}
