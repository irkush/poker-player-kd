# Simple Nancy App

A bare-bones, self-hosting Nancy app with instructions for Heroku deployment.
All of the heavy lifting was done by [Ben Hall](https://github.com/BenHall); the
goal of this fork is simply to collect and document Ben's hard work to make
simple C# apps on Heroku even easier to create:

```shell
$ git clone https://github.com/dvdsgl/nancy-simple-heroku.git
$ cd nancy-simple-heroku
$ rake init
```

That's all you have to do to get a C# web app deployed on Heroku.

## Dependencies

First off, you need to [get
Mono](http://www.go-mono.com/mono-downloads/download.html), create a
[Heroku account](https://api.heroku.com/signup), and install the [Heroku toolbelt](https://toolbelt.heroku.com/)
if you haven't already. Then, to create a new Heroku app, deploy your
app to Heroku, and open it in your browser, simply do:

```shell
$ rake init
```

You should only run `rake init` once.  When Heroku creates a new app, it
assigns a unique hostname to your app–something like
`stark-hollows-8350.herokuapp.com`, for example. `rake init` reads this
hostname and saves it in a Heroku environment variable named `HOST`,
which the Nancy app reads to bind to the correct URL. If you ever change
your Heroku host name, remember to update `HOST`:

```shell
$ heroku config:add HOST=<your new hostname>
```

## Building Locally

```shell
$ rake build
```

## Running Locally

```shell
$ rake stage # or just `rake`
```

## Deploying to Heroku

This is merely a shortcut for `git push heroku master`:

```shell
$ rake deploy
```
