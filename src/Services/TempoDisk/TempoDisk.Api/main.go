package main

import (
	"fmt"

	"tempodisk/server"
)

func handlerPanic() {
	if r := recover(); r != nil {
		fmt.Println(r)
	}

	fmt.Println("End a panic recovering")
}

func main() {
	defer handlerPanic()

	server := server.New()
	err := server.Run()

	if err != nil {
		fmt.Println(err)
		return
	}
}
