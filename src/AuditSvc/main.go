package main

import (
	"context"
	"fmt"
	"log"
	"os"
	"strings"

	dapr "github.com/dapr/go-sdk/client"
	"github.com/dapr/go-sdk/service/common"
	daprd "github.com/dapr/go-sdk/service/grpc"
)

func getEnvVar(key, fallbackValue string) string {
	if val, ok := os.LookupEnv(key); ok {
		return strings.TrimSpace(val)
	}
	return fallbackValue
}

var (
	logger         = log.New(os.Stdout, "== AuditSvc == ", 0)
	serviceAddress = getEnvVar("ADDRESS", ":50001")
	pubSubName     = getEnvVar("PUBSUB_NAME", "audit")
	topicName      = getEnvVar("TOPIC_NAME", "messages")
)

func main() {
	// create Dapr service
	s, err := daprd.NewService(serviceAddress)
	if err != nil {
		logger.Fatalf("failed to start the server: %v", err)
	}

	// add handler to the service
	subscription := &common.Subscription{
		PubsubName: pubSubName,
		Topic:      topicName,
	}
	if err := s.AddTopicEventHandler(subscription, eventHandler); err != nil {
		logger.Fatalf("error adding handler: %v", err)
	}

	// start the server to handle incoming events
	if err := s.Start(); err != nil {
		logger.Fatalf("server error: %v", err)
	}
	logger.Println("AuditSvc started")
}

func eventHandler(ctx context.Context, e *common.TopicEvent) (retry bool, err error) {
	msg := fmt.Sprintf("event - PubsubName: %s, Topic: %s, ID: %s, Data: %s", e.PubsubName, e.Topic, e.ID, e.Data)
	logger.Println(msg)

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
	logger.Printf("saving data: %s\n", string(data))
	if err := client.SaveState(ctx, store, "auditkey", data); err != nil {
		panic(err)
	}
	logger.Println("data saved")

	return false, nil
}
