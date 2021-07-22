package main

import (
	"context"
	"fmt"
	"log"
	"net/http"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/dapr/go-sdk/service/common"
	daprd "github.com/dapr/go-sdk/service/http"
)

// Subscription to tell the dapr what topic to subscribe.
// - PubsubName: is the name of the component configured in the metadata of pubsub.yaml.
// - Topic: is the name of the topic to subscribe.
// - Route: tell dapr where to request the API to publish the message to the subscriber when get a message from topic.
var sub = &common.Subscription{
	PubsubName: "messages",
	Topic:      "audit",
	Route:      "/audit",
}

func main() {
	s := daprd.NewService(":8080")

	if err := s.AddTopicEventHandler(sub, eventHandler); err != nil {
		log.Fatalf("error adding topic subscription: %v", err)
	}

	if err := s.Start(); err != nil && err != http.ErrServerClosed {
		log.Fatalf("error listenning: %v", err)
	}
}

func eventHandler(ctx context.Context, e *common.TopicEvent) (retry bool, err error) {
	msg := fmt.Sprintf("event - PubsubName: %s, Topic: %s, ID: %s, Data: %s", e.PubsubName, e.Topic, e.ID, e.Data)
	log.Print(msg)

	client, err := dapr.NewClient()
	if err != nil {
		panic(err)
	}
	defer client.Close()
	//TODO: use the client here, see below for examples

	store := "statestore"
	json := fmt.Sprintf(`{ "data": "%s" }`, e.Data)
	data := []byte(json)

	// save state with the key key1
	fmt.Printf("saving data: %s\n", string(data))
	if err := client.SaveState(ctx, store, "auditkey", data); err != nil {
		panic(err)
	}
	fmt.Println("data saved")

	return false, nil
}
