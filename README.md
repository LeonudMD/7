# Команды для кафки
### Проверить, есть ли сообщения в топике:
- docker exec -it eventureapi-kafka-1 kafka-console-consumer --bootstrap-server kafka:9092 --topic user-registered-topic --from-beginning

### Отправить тестовое сообщение в топик:
- docker exec -it eventureapi-kafka-1 kafka-console-producer --bootstrap-server kafka:9092 --topic user-registered-topic

### Создать топик вручную:
- docker exec -it eventureapi-kafka-1 kafka-topics --bootstrap-server kafka:9092 --create --topic user-registered-topic --partitions 1 --replication-factor 1
- docker exec -it eventureapi-kafka-1 kafka-topics --bootstrap-server kafka:9092 --create --topic registration-response-topic --partitions 1 --replication-factor 1
### Посмотреть доступные топики в Kafka:
- docker exec -it eventureapi-kafka-1 kafka-topics --bootstrap-server kafka:9092 --list

