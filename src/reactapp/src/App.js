import React, { Component } from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { useState } from 'react';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = { forecasts: [], loading: true, message: "", updated: "" };

        this.handleClick = this.handleClick.bind(this);
        this.handleChange = this.handleChange.bind(this);

    }

    handleChange(event) {
        this.setState({ message: event.target.value });
    }

    async handleClick() {
        // "message" stores input field value
        // test for git
        //this.setState({ updated: this.state.message });
        //this.populateWeatherData();
        this.setState({ loading: true });

        let policy = {
            conditionalAccessPolicyJson: this.state.message
        };

        const options = {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify(policy)
        };

        fetch('weatherforecast', options)
            .then((response) => response.blob())
            .then((blob) => {

                // 2. Create blob link to download
                const url = window.URL.createObjectURL(new Blob([blob]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', `CA Policy.pptx`);
                // 3. Append to html page
                document.body.appendChild(link);
                // 4. Force download
                link.click();
                // 5. Clean up and remove the link
                link.parentNode.removeChild(link);
                this.setState({
                    loading: false
                });
            })
            .catch((error) => {
                error.json().then((json) => {
                    this.setState({
                        errors: json,
                        loading: false
                    });
                })
            });
    }

    download(blob, filename) {
        const url = window.URL.createObjectURL(new Blob([blob]));
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = url;
        // the filename you want
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }

    componentDidMount() {
        //this.populateWeatherData();
    }

    static renderForecastsTable(forecasts) {
        return (
            <>

                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Temp. (C)</th>
                            <th>Temp. (F)</th>
                            <th>Summary</th>
                        </tr>
                    </thead>
                    <tbody>
                        {forecasts.map(forecast =>
                            <tr key={forecast.date}>
                                <td>{forecast.date}</td>
                                <td>{forecast.temperatureC}</td>
                                <td>{forecast.temperatureF}</td>
                                <td>{forecast.summary}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
            : App.renderForecastsTable(this.state.forecasts);

        return (
            <div>
                <h1 id="tabelLabel" >Weather forecast</h1>
                <p>This component demonstrates fetching data from the server.</p>

                <div>
                    <input
                        type="text"
                        id="message"
                        name="message"
                        onChange={this.handleChange}
                        value={this.state.message}
                    />

                    <button onClick={this.handleClick}>Update</button>
                </div>
                <label>{this.state.updated}</label>
                {contents}
            </div>
        );
    }

    async populateWeatherData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        this.setState({ forecasts: data, loading: false });
    }
}
