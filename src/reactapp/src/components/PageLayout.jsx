import React from "react";
import { NavBar } from "./NavBar"
import { Login } from '@microsoft/mgt-react';
import { Link, Outlet } from "react-router-dom";
import { SocialIcon } from 'react-social-icons';
import { envConfig } from "../authConfig"

export const PageLayout = (props) => {
    return (
        <>
            <div id="container">
                <header>

                    <div className="logo" role="presentation" aria-hidden="true" itemProp="logo" itemScope="itemScope">
                        envConfig.envName<img src="/idPowerToysLogo.png" height="46" alt="Icon of woman with a pumped up arm showing a can do attitude."></img>

                    </div>
                    <div className="colorgroup"></div>
                    <div className="profile">
                        <Login />
                    </div>
                </header>

                <main>
                    <nav>
                        <NavBar />
                    </nav>
                    <div className="content">
                        <article id="article">
                            <Outlet />
                            {/* {props.children} */}
                        </article>
                        <aside>
                        </aside>
                    </div>
                </main>
                <footer>
                    <div className="switches">
                        <small>
                            <SocialIcon url="https://twitter.com/merill" style={{ height: 20, width: 20 }} />&nbsp;
                            <SocialIcon url="https://www.linkedin.com/in/merill/" style={{ height: 20, width: 20 }} />&nbsp;
                            <SocialIcon url="https://github.com/merill" style={{ height: 20, width: 20 }} />&nbsp;
                        </small>
                    </div>
                    <div className="version">
                    </div>
                    <div className="copy">
                        <small>
                            <Link to="/about">About</Link>
                            &nbsp;|&nbsp;<a target="_blank" rel="noreferrer" href="/privacy.html">Privacy Policy</a>
                        </small>
                    </div>
                </footer>
            </div>
        </>
    );
};