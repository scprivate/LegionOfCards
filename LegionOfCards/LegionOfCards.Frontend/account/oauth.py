import requests
import mysql.connector

class Oauth(object):
    loc_user_id = ""

    client_id = "565316343637737497"
    client_secret = "7pTt97X9D0-uxJR0N0y48_i9nfY_yARA"
    scope = "identify%20email"
    redirect_uri = "http://127.0.0.1:5000/discord/auth/success"
    discord_auth_url = "https://discordapp.com/api/oauth2/authorize?client_id={}&redirect_uri={}&response_type=code&scope={}".format(client_id, redirect_uri, scope)
    discord_token_url = "https://discordapp.com/api/oauth2/token"
    discord_api_url = "https://discordapp.com/api"

    @staticmethod
    def get_access_token(code):
        payload = {
            'client_id': Oauth.client_id,
            'client_secret': Oauth.client_secret,
            'grant_type': 'authorization_code',
            'code': code,
            'redirect_uri': Oauth.redirect_uri,
            'scope': Oauth.scope
        }
        headers = {
            'Content-Type': 'application/x-www-form-urlencoded'
        }

        access_token = requests.post(url = Oauth.discord_token_url, data = payload, headers = headers)
        json = access_token.json()
        return json.get("access_token")

    @staticmethod
    def get_user_data(access_token):
        url = Oauth.discord_api_url + "/users/@me"
        headers = {
            "Authorization": "Bearer {}".format(access_token)
        }

        user_json = requests.get(url = url, headers = headers)
        user_data = user_json.json()
        return user_data

    @staticmethod
    def insert_user_id(discord_id):
        mydb = mysql.connector.connect(
            host = "legionofsensei.de",
            user = "das ist nicht der richtige user",
            passwd = "das ist nicht das richtige passwort",
            database = "das ist nicht die richtige db"
        )
        mycursor = mydb.cursor()
        sql = "UPDATE loc__users SET DiscordID = %s WHERE UserID = %s"
        val = (discord_id, Oauth.loc_user_id)
        mycursor.execute(sql, val)
        mydb.commit()

        headers = {
            "User": Oauth.loc_user_id
        }
        requests.get(url = "http://localhost:25325/discord/verify/success", headers = headers)