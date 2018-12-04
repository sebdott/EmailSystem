# Emailing System

Emailing System is an email system which sends emails to users once everyday with certain rules.

This is the list of technologies used :- 

**Technologies:-**

- **Front End Javascript** : React (https://reactjs.org/)
- **Rest API** : .NET Core 2.0.5 , C#
- **Container**: Docker (https://www.docker.com/)
- **Log Management** : Graylog (https://www.graylog.org)
- **DB**: Mongo DB  (https://www.mongodb.com/)
      Elastic Search - Graylog  (www.elastic.co/)
- **PubSub/Message/Stream** : Kafka (https://www.confluent.io/)
                          ZooKeeper (https://zookeeper.apache.org/)
- **Docker Management UI** : Portainer (https://portainer.io/)


This is a rough idea of the process of this system.
![alt text](https://raw.githubusercontent.com/codedsphere/EmailingSystem/master/Images/SystemFlow.jpg)

**Tools Used:-**
- Visual Studio 2017
- Docker

**Installation Instruction**

These are the few steps required to set up all the environments in your machine.

First of all you need docker to be set up in your machine. 
For me I am running on Windos OS which my docker containers will be running on Linux settings
You can go to (https://www.docker.com/community-edition) to get the latest version of docker downloads

Go to powershell

Run this command to bring up all the applications and environments in docker
```
docker-compose up -d
```

Once the environment is being brought up you will have the list of application running on your machine.

**Local Path/Ports** : -

- **Front End UI** : localhost:802
- **Web Api** : localhost:801/swagger
- **GrayLog** : localhost:9038

I have ported the necessary port numbers to your machine ports
If your machine have occupied these port numbers you may change it in the docker compose config file (docker-compose.yml) 

```
Find the line -p <public port>:<private port> and do the necessary modifications.
```

**Application Screenshots:-**

**Front End UI :** 
![alt text](https://raw.githubusercontent.com/codedsphere/EmailingSystem/master/Images/FrontUI.JPG)

**Back End Api Swagger:** 
![alt text](https://raw.githubusercontent.com/codedsphere/EmailingSystem/master/Images/BackEnd.JPG)

**GrayLog : Log Management :**  
```
link : localhost:9038 
username : admin 
password : admin
```

