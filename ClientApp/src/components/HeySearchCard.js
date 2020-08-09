import React from 'react';
import moment from 'moment';
import styled from 'styled-components'
import DOMPurify from 'dompurify';

import ExternalLink from './ExternalLink';

const Styles = styled.div`
    padding: 1rem;
    .cards_container {
        display: flex;
        flex-flow: column;
        width: 100%;
        text-align: center;
    }
    .card_container {
        display: flex;
        flex-flow: column;
        margin: 1rem auto;
        border: 1px solid gray;
        border-radius:5px; 
        width: 100%;
    }
    .card_header {
        display: flex;
        flex-flow: row nowrap;
        justify-content: space-between;
        margin: 0.5rem;
        border-bottom: 1px dotted gray;
    }
    .card_content {
        text-align: left;
        margin: 0.5rem;
    }
    .card_images {
        display: flex;
        flex-flow: row wrap;
        align-content: flex-start;
        justify-content: space-around;

    }
    .card_reference {
        text-align: right;
        margin: 0.5rem;
    }
    .image_preview {
        margin: 0.5rem;
        width: 200px;
        
    }
`;

const HeySearchCard = (props) => {
    const { data } = props;

    const cards = data.map((item) => {
        const day = moment(item.created);
        

        // making sure that the tweet content is sanitize
        // because dangerouslySetInnerHTML is used to embedded a link in the text
        let tweet = DOMPurify.sanitize(item.content);
        tweet = tweet.replace(
            /((http|https):\/\/[a-zA-Z0-9\-.]+\.[a-zA-Z]{2,3}(\/\S*)?)/g,
            '<a href="$1" target="_blank" rel="noopener noreferrer">$1</a>'
        );

        return (
            <div key={item.id} className="card_container">
                <div className="card_header">
                    <div>{day.format('D-MMM-YY')} {day.format('HH:mm')}</div>
                    <div>posted by <ExternalLink href={`https://twitter.com/${item.userName}`}>{item.userName}</ExternalLink></div>
                </div>
                <div className="card_content">
                    <span dangerouslySetInnerHTML={{__html: tweet}} />
                    <span>
                        &nbsp;
                        <ExternalLink href={`https://twitter.com/${item.userName}/status/${item.id}`}>
                            <img width="15" src="/external_link.png" alt="open" />
                        </ExternalLink>
                    </span>
                </div>
                <div className="card_images">
                {
                    item.imageUrls.map(url => {
                        return (
                            <ExternalLink href={url} key={url}>
                                <img className="image_preview" src={url} alt={url} />
                            </ExternalLink>
                        );
                    })
                }
                </div>
                <div className="card_reference">
                {
                    (item.originalId ? <ExternalLink href={`https://twitter.com/${item.originalUserId}/status/${item.originalId}`}>Referenced Tweet</ExternalLink> : null)
                }
                </div>
            </div>
        );
    });

    return (
        <Styles>
            <div className="cards_container">
                {cards}
            </div>
        </Styles>
        
    );
};

export default HeySearchCard;