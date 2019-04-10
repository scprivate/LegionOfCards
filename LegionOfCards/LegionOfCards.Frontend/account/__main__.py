from flask import Flask, request, render_template, redirect, session
from oauth import Oauth

app = Flask(__name__)

@app.route("/discord/verify", methods = ["get"])
def index():
    Oauth.loc_user_id = request.args.get("user_id")
    print(Oauth.loc_user_id)
    if Oauth.loc_user_id:
        return redirect(Oauth.discord_auth_url)
    return "400 Bad Request"

@app.route("/discord/auth/success", methods = ["get"])
def auth():
    code = request.args.get("code")
    access_token = Oauth.get_access_token(code)
    user = Oauth.get_user_data(access_token)
    Oauth.insert_user_id(user.get("id"))
    return "Success!"

if(__name__ == "__main__"):
    app.run(debug=True)