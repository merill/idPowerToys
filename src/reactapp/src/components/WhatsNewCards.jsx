import {
    makeStyles,
    shorthands,
    Button,
    Caption1,
    Body1,
    Subtitle1,
} from "@fluentui/react-components";
import {
    Card,
    CardHeader,
    CardFooter,
    CardPreview,
    CardProps,
} from "@fluentui/react-components/unstable";
import { useNavigate } from "react-router-dom";

import {
    SlideLayout24Filled,
} from "@fluentui/react-icons";
import * as React from "react";

const useStyles = makeStyles({
    main: {
        display: "flex",
        flexWrap: "wrap",
        flexDirection: "column",
        columnGap: "16px",
        rowGap: "36px",
    },

    title: {
        ...shorthands.margin(0, 0, "12px"),
    },

    description: {
        ...shorthands.margin(0, 0, "12px"),
    },

    card: {
        width: "400px",
        maxWidth: "100%",
        height: "fit-content",
    },

    text: {
        ...shorthands.margin(0),
    },
});

const Header = ({ title, description }) => {
    const styles = useStyles();

    return (
        <>
            {title ? (
                <Subtitle1 as="h4" block className={styles.title}>
                    {title}
                </Subtitle1>
            ) : null}

            {description ? (
                <Body1 as="p" block className={styles.description}>
                    {description}
                </Body1>
            ) : null}
        </>
    );
};

const CardExample = (props) => {
    const styles = useStyles();
    const navigate = useNavigate();

    return (
        <Card className={styles.card} {...props}>
            <CardPreview>
                <img
                    src="./assets/whatsnew/cadocgen.png"
                    alt="Conditional Acccess Document Generated in PowerPoint."
                />
            </CardPreview>

            <CardHeader
                header={
                    <Body1>
                        <b>Conditional Access Documenter</b>
                    </Body1>
                }
            />

            <p className={styles.text}>
                Export your conditional access policies to PowerPoint for a bird's eye view of your security posture. Share you policies with security teams and stakeholders without giving admin access to Azure Active Directory.</p>
            <p className={styles.text}>
                Your conditional access policies will never look the same again.
            </p>

            <CardFooter>
                <Button appearance="primary" icon={<SlideLayout24Filled />}
                    onClick={() => navigate('/ca')}
                >
                    Try it out
                </Button>
            </CardFooter>
        </Card>
    );
};

export const WhatsNewCards = () => {
    const styles = useStyles();

    return (
        <div className={styles.main}>
            <section>
                <Header
                    title="Welcome to idPowerToys"
                    description="idPowerToys is a collection of mini-apps that I've been building (planning to build) that are related to Microsoft Entra, Azure Active Directory and Microsoft Graph. I hope you find them useful."
                />

                <CardExample />
            </section>
        </div>
    );
};
