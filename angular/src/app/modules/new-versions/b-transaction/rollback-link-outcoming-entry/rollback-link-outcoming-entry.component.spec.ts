import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RollbackLinkOutcomingEntryComponent } from './rollback-link-outcoming-entry.component';

describe('RollbackLinkOutcomingEntryComponent', () => {
  let component: RollbackLinkOutcomingEntryComponent;
  let fixture: ComponentFixture<RollbackLinkOutcomingEntryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RollbackLinkOutcomingEntryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RollbackLinkOutcomingEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
