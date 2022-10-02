package abstractions

import "context"

type Pool interface {
	Start(ctx context.Context)
	Wait()
}
