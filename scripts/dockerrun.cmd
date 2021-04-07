docker run --name valuatorRedis -p 6379:6379 -d redis
docker run --name valuatorRedisRU -p 6000:6379 -d redis
docker run --name valuatorRedisEU -p 6001:6379 -d redis
docker run --name valuatorRedisOther -p 6002:6379 -d redis
docker run --name valuatorNats -p 4222:4222 -p 6222:6222 -p 8222:8222 -d nats