
# Projet : Let's go Biking !

## Description

Ce projet consiste en le développement d’un serveur SOAP et REST auto-hébergé (avec cache et proxy) en C#.

Le Server Proxy/Cache interagit avec l'API de JC Decaux pour obtenir la localisations des les informations des stations de vélo, et l'API Open Route Service pour planifierl'itineraire.

Coté client, l'application lui permet de demander un itineraire entre 2 localisations et de l'afficher.

## Fonctionnalités

- Entrez n’importe quel lieu d’origine et de destination en coordonée GPS, en nom de rue ou en position actuelle
- Visualisation de l’itinéraire sur une carte avec différenciation  entre les passages à pied ou a vélo
- Interface graphique complète et responsive
- Le voyage est organisé de votre position actuelle à la station la plus proche, puis de la station la plus proche de votre destination à votre destination finale
- Serveur SOAP et REST auto-hébergé en C# pour la planification d’itinéraires
- Intégration avec l’API JCDecaux dont d'autres API REST externes
- Fonctions avancées comme le cache et la gestion des files d’attente avec ActiveMQ 

## Installation

/!\ Windows requis

Une fois le dossier téléchargé, veuillez ouvrir le fichier RoutingServer.sln situé dans middleware/MiddlewareServers pour ouvrir la solution dans votre IDE en tant qu'administrateur. Vérifiez que toutes les dépendances sont correctes.

Ensuite veuillez générer la solution.
Pour lancer le projet sans éditeur : 

lancez dans middlewareServers : 
- [\apache-activemq-5.18.5\bin\win64\activemq.bat](/middlewareServers/apache-activemq-5.18.5/bin/win64/activemq.bat)
- [\ProxyCacheServer\bin\Debug\ProxyCacheServer.exe](/middlewareServers/ProxyCacheServer/bin/Debug/ProxyCacheServer.exe)
- [\RoutingServer\bin\Debug\RoutingServer.exe](/middlewareServers/RoutingServer/bin/Debug/RoutingServer.exe)

un .bat est également disponible pour automatiser ces actions ([start.bat](/middlewareServers/start.bat))


## Comment utiliser

Une fois l'application lancée, vous pouvez rentrer une addresse de départ dans la barre de recherche départ et une addresse d'arrivée dans la barre de recherche correspondante. Pour choisir comme position de départ sa position actuelle, il faut commencer à rentrer une addresse puis choisir "Ma Position" dans les propositions d'autocompletions.

Vous pouvez également cliquer sur le champs souhaité (départ/destionation) puis appuyer sur un emplacement de la carte pour choisir cette emplacement.

Une fois les deux adresses entrée, l'application va calculer le chemin le plus court entre tout à pied ou utiliser un vélo depuis une station JC Decaux, et l'afficher. En bas de la page vous aurez accès à l'instruction actuelle et sur le coté droit, la liste des instructions.

## Auteurs

- Erwan HESCHUNG et Maxence ROQUES
