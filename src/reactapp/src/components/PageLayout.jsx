import React from "react";
import { NavBar } from "./NavBar"
import { Outlet } from "react-router-dom";
import { Login } from '@microsoft/mgt-react';

export const PageLayout = (props) => {
    return (
        <>
            <div id="container">
                <header>
                    
                    <div className="logo" role="presentation" aria-hidden="true" itemProp="logo" itemScope="itemScope">
                        <img src="/idPowerToysLogo.png" height="46"></img>    
                    </div>
                    <div className="colorgroup"></div>
                    <div className="profile">
                        {/* {isAuthenticated ? <SignOutButton /> : <SignInButton />} */}
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
                        
                    </div>
                    <div className="version">
                        <small>
                            <a target="_blank" href="https://icons8.com/icon/1CbCOtKH87xx/we-can-do-it">We Can Do It</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>&nbsp;
                            |&nbsp;Presentation Components by <a target="_blank" href="https://www.syncfusion.com/">Syncfusion</a>
                        </small>
                    </div>
                    <div className="copy">
                        <small>Brought to you by <a target="_blank" href="https://merill.net/">Merill</a></small>
                    </div>
                </footer>
            </div>
        </>
    );
};